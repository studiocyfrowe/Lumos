import pyodbc

def get_connection():
    return pyodbc.connect(
        "DRIVER={ODBC Driver 18 for SQL Server};"
        "SERVER=DESKTOP-RJ92I5V;"
        "DATABASE=MonitorDB;"
        "Trusted_Connection=yes;"
        "MultipleActiveResultSets=True;"
        "TrustServerCertificate=True;"
    )
