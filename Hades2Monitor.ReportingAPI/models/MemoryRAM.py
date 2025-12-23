from datetime import datetime
from pydantic import BaseModel

class MemoryRAM(BaseModel):
    total_gb: float
    used_gb: float
    free_gb: float
    used_percent: float
    last_scan: datetime
