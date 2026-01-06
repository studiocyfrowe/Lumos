import pyodbc
from typing import Callable, List, TypeVar, Any

T = TypeVar("T")

class QueryHelper:
    def __init__(self, connection_string: str):
        self._connection_string = connection_string

    def get_connection(self):
        return pyodbc.connect(self._connection_string)