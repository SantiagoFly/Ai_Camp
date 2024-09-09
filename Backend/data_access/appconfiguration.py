from azure.appconfiguration import AzureAppConfigurationClient

# Configura la conexión al servicio Azure App Configuration
connection_str = "Endpoint=https://ac-camposol-poc-paltas-peru.azconfig.io;Id=PAck;Secret=gzzaKQPss3Opn3nZrOk3PwO6IdkzqzRQhXrwFfNgjw0="
config_client = AzureAppConfigurationClient.from_connection_string(connection_str)

# Función para obtener una configuración
def get_config(key):
    try:
        fetched_config = config_client.get_configuration_setting(key)
        return fetched_config.value
    except Exception as e:
        print(f"Error al obtener la configuración para {key}: {e}")
        return None

