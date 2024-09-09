import os
from azure.cosmos import CosmosClient, exceptions, DatabaseProxy
from azure.cosmos.partition_key import PartitionKey
from data_access.appconfiguration import get_config

DATABASE_THROUGHPUT = 1000

COSMOS_CONNECTION_STRING = get_config("cosmos.connectionString")
DATABASE_ID = get_config("cosmos.database_id")
CONTAINER_ID = get_config("cosmos.container_id")
PARTITION_KEY = get_config("cosmos.partition_key")

class CosmosDBDataAccess():
    _client: CosmosClient = None
    _database: DatabaseProxy = None

    def __init__(self) -> None:
        raise RuntimeError('Call get_client() instead')
    
    @classmethod
    def get_database(cls):
        """ Get database object based on settings"""
        if cls._client is None:
            cls._client = CosmosClient.from_connection_string(COSMOS_CONNECTION_STRING)
            cls._database = cls._client.create_database_if_not_exists(id=DATABASE_ID)
        return cls._database

    @classmethod
    def get_container(cls, container_id: str, partition_key: str):
        """ Get container by Id """
        database = cls.get_database()
        try:
            container = database.create_container(id=container_id, partition_key=PartitionKey(path=partition_key))
        except exceptions.CosmosResourceExistsError:
            container = database.get_container_client(container_id)

        return container