import logging
import time
from azure.storage.blob import BlobServiceClient
from azure.core.exceptions import ResourceExistsError
from data_access.appconfiguration import get_config

STORAGE_CONNECTION_STRING = get_config("blobstorage.connectionString")

class BlobStorageDataAccess():
    """ Class that handles the access to Blob Storage"""
    _blob_service_client: BlobServiceClient = None
    @classmethod
    def get_client(cls):
        """ Gets a Blob Service client object"""
        if cls._blob_service_client is None:
            cls._blob_service_client = BlobServiceClient.from_connection_string(STORAGE_CONNECTION_STRING)

        return cls._blob_service_client

    @classmethod
    def get_container_client(cls, container_name: str):
        """ Gets a container client """
        blob_service_client = cls.get_client()
        try:
            container_client = blob_service_client.create_container(name=container_name)
        except ResourceExistsError:
            logging.info(f'A container with the name {container_name} already exists')
            container_client = blob_service_client.get_container_client(container=container_name)
        return container_client

    @classmethod
    def upload_file(cls, container_name: str, file_name: str, file_data: bytes):
        """ Uploads a file to a specific container """
        container_client = cls.get_container_client(container_name=container_name)
        blob_client = container_client.get_blob_client(blob=file_name)
        blob_client.upload_blob(file_data)

    @classmethod
    def move_file(cls, source_container: str, destination_container: str, file_name: str):
        """ Moves a file from one container to another """
        source_container_client = cls.get_container_client(source_container)
        destination_container_client = cls.get_container_client(destination_container)

        # Get the source blob client
        source_blob_client = source_container_client.get_blob_client(blob=file_name)

        # Get the destination blob client
        destination_blob_client = destination_container_client.get_blob_client(blob=file_name)

        # Start the copy and obtain the copy ID for status checking
        try:
            copy = destination_blob_client.start_copy_from_url(source_blob_client.url)
            copy_id = copy['copy_id']
            logging.info(f"Copy started with ID: {copy_id}")

            # Poll for the copy status
            while True:
                time.sleep(1)
                blob_properties = destination_blob_client.get_blob_properties()
                copy_status = blob_properties.copy.status
                logging.info(f"Checking copy status: {copy_status}")

                if copy_status == 'success':
                    logging.info(f"Copy succeeded, deleting source blob: {file_name}")
                    source_blob_client.delete_blob()                    
                    logging.info(f"Successfully moved {file_name} from {source_container} to {destination_container}")
                    break
                elif copy_status == 'failed':
                    logging.error(f"Failed to copy {file_name}: Status={copy_status}")
                    break
                # Optionally handle other statuses like 'aborted', 'pending', etc.

        except Exception as e:
            logging.error(f"Error during file move: {str(e)}")