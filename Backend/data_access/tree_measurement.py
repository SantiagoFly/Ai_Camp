import uuid
from pydantic import BaseModel
from data_access.cosmosdb import CosmosDBDataAccess
from azure.cosmos import exceptions
from typing import List
import logging
from data_access.appconfiguration import get_config


CONTAINER_ID = get_config("cosmos.container_id")
PARTITION_KEY = get_config("cosmos.partition_key")



class TreeMeasurement(BaseModel):
    id: str = None
    partitionKey: str = None
    tree_group: str
    tree_row: str
    tree_plant: str
    avocado_measurements: List[List[float]] = []
    is_even: bool = True
    all_three_digits: bool = True
    name_file: str

    @staticmethod
    def get_by_id(id: str):
        """ Get a Tree measurement item by Id"""
        container = CosmosDBDataAccess.get_container(CONTAINER_ID, PARTITION_KEY)
        try:
            data = container.read_item(item=id, partition_key=id)
            item = TreeMeasurement(**data)
        except exceptions.CosmosHttpResponseError as e:
            logging.error(e)
            return None
        return item


    def store(self):
        """ Inserts or update the current instance in the database"""
        try:
            container = CosmosDBDataAccess.get_container(CONTAINER_ID, PARTITION_KEY)
            if (self.id is None):
                self.id = str(uuid.uuid4())
                self.partitionKey = self.id
                data = self.model_dump()
                container.create_item(body=data)
            else:
                data = self.model_dump()
                read_item = container.read_item(item=self.id, partition_key=self.partitionKey)
                item_etag = read_item["_etag"]
                # Update item values
                read_item.update( (key, data[key]) for key in read_item if key in data)
                container.replace_item(read_item, read_item, if_match=item_etag)
        except exceptions.CosmosHttpResponseError as e:
            logging.error(e)
            return False
        return self
