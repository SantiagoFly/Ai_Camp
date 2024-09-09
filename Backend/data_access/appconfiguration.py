from azure.appconfiguration import AzureAppConfigurationClient
from data_access.appconfiguration import get_config

# Configura la conexión al servicio Azure App Configuration

CCONNECTION_STRING = get_config("connectionString")
config_client = AzureAppConfigurationClient.from_connection_string(CCONNECTION_STRING)

# Función para obtener una configuración
def get_config(key):
    try:
        fetched_config = config_client.get_configuration_setting(key)
        return fetched_config.value
    except Exception as e:
        print(f"Error al obtener la configuración para {key}: {e}")
        return None

