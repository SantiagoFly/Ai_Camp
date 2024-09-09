""" Module Doc String """
import time
import os
from typing import List
import logging
import tempfile
import azure.functions as func
from fastapi import Depends, FastAPI, Request, Response, UploadFile
from pydantic import BaseModel 
from data_access.blobstorage import BlobStorageDataAccess
from data_access.tree_measurement import TreeMeasurement
import azure.cognitiveservices.speech as speechsdk
from data_access.cosmosdb import CosmosDBDataAccess
from data_access.appconfiguration import get_config
import datetime

fast_app = FastAPI()

FUNCTIONS_WORKER_RUNTIME = "python"
app = func.AsgiFunctionApp(app=fast_app, http_auth_level=func.AuthLevel.FUNCTION)

class TreeDefinition(BaseModel):
    lote: str
    hilera: str
    planta: str

@fast_app.post("/api/upload_audio")
async def upload_audio(req: Request, definition: TreeDefinition = Depends(), audio_file: UploadFile = None) -> Response:
    """ Doc String"""
    logging.info('Processing upload audio request.')

    # Crear registro de medición en cosmosdb con datos de indentificación recibidos y medidas vacías
    measurement = TreeMeasurement(tree_group=definition.lote, tree_row=definition.hilera, tree_plant=definition.planta, avocado_measurements = [])
    measurement.store()

    # ALmacenar audio en container de blob storage usando como nombre el id del registro de medición
    file_data = await audio_file.read()
    BlobStorageDataAccess.upload_file("audio-files", measurement.id, file_data)

    logging.info(f"Se almacenó el audio para el registro con el código {measurement.id}")

    return Response("Audio almacenado correctamente", status_code=200)




@app.blob_trigger(arg_name="audio", path="audio-files/{filename}", connection= "AzureWebJobsStorage")
def audio_file_blob_trigger(audio: func.InputStream):
    logging.info(f"Processing blob trigger for file: {audio.name}")

    audio_name = audio.name.split('/')[-1]
    parts = audio_name.split('-')
    if len(parts) >= 4:
        lote = parts[0]
        hilera = parts[1]
        planta = parts[2]

    fecha2 = datetime.datetime.now() 
    # Convertir el objeto datetime a una cadena en el formato deseado 

    
    fecha = fecha2.strftime("%Y-%m-%d %H:%M:%S")
    logging.info(f"Lote: {lote}, Hilera: {hilera}, Planta: {planta}, Fecha: {fecha} ")


    speech_key = get_config("SpeechServiceKey")
    speech_region = get_config("SpeechServiceRegion")
    speech_config = speechsdk.SpeechConfig(subscription=speech_key, region=speech_region)

    try:
        with tempfile.NamedTemporaryFile(delete=False) as temp_audio_file:
            temp_audio_file.write(audio.read())
            temp_audio_file_path = temp_audio_file.name

        logging.info(f"Temporary file created as {temp_audio_file_path}")
        audio_input = speechsdk.AudioConfig(filename=temp_audio_file_path)
        speech_recognizer = speechsdk.SpeechRecognizer(speech_config=speech_config, language="es-ES",audio_config=audio_input)

        all_results = []
        done = False

        def stop_cb(evt):
            """Callback that stops continuous recognition upon receiving an event `evt`."""
            print('CLOSING on {}'.format(evt))
            speech_recognizer.stop_continuous_recognition()
            nonlocal done
            done = True        

        # Connect callbacks to the events fired by the speech recognizer
        speech_recognizer.recognized.connect(lambda evt: all_results.append(evt.result.text))
        speech_recognizer.session_stopped.connect(stop_cb)

        # Start continuous speech recognition
        speech_recognizer.start_continuous_recognition()
        while not done:
            time.sleep(.5)

        

    except Exception as e:
        logging.error(f"Failed to process audio: {str(e)}")

    finally:
        if os.path.exists(temp_audio_file_path):
            os.remove(temp_audio_file_path)
            logging.info("Temporary file deleted.")
    

    if all_results:
        transcription = " ".join(all_results)
        logging.info(f"Recognized audio as: {transcription}")
        
        transformed_transcription, even_flag, three_digits_flag = transform_transcription(transcription)
        logging.info(f"Recognized transformed : {transformed_transcription}")
        logging.info(f"all elements are in pairs: {even_flag}")
        logging.info(f"all elements are of 3 digits: {three_digits_flag}")

        BlobStorageDataAccess.move_file("audio-files", "audio-files-processed", audio_name)

        measurements = transformed_transcription

        measurement = TreeMeasurement(tree_group=lote, tree_row=hilera, tree_plant=planta, avocado_measurements = measurements, is_even = even_flag, all_three_digits = three_digits_flag, date_time = fecha, name_file = audio_name)
        measurement.store()
        
        logging.info(f"Audio file processed successfully: Measurements: {measurements}")
        logging.info(f"Is_even flag: {even_flag}")
        logging.info(f"All_three_digits flag: {three_digits_flag}")

    


def transform_transcription(transcription: str):
    # Eliminar espacios en blanco y dividir por "x" para obtener los números
    numbers = transcription.replace(" ", "").replace("X", "x").split("x")
    
    # Filtrar elementos vacíos que puedan resultar del proceso anterior
    numbers = list(filter(None, numbers))
    
    # Limpiar caracteres no numéricos y convertir los elementos a enteros
    cleaned_numbers = []
    all_three_digits = True # Asumimos que todos los números tienen 3 dígitos
    for num in numbers:
        # Eliminar todos los caracteres no numéricos
        cleaned_num = ''.join(filter(str.isdigit, num))
        if cleaned_num:  # Asegurarse de que no esté vacío
            if len(cleaned_num) != 3:
                all_three_digits = False  # Encontrar un número que no tenga tres cifras
            cleaned_numbers.append(int(cleaned_num))

    
    # Agrupar los números en pares
    pairs_of_three_digits = []
    is_even = True  # Asumimos que la cantidad de números es par inicialment
    for i in range(0, len(cleaned_numbers), 2):
        if i+1 < len(cleaned_numbers):
            pair = [cleaned_numbers[i], cleaned_numbers[i+1]]
            pairs_of_three_digits.append(pair)
            
        else:
            # Si nos quedamos con un número suelto, cambiamos la bandera a False
            pairs_of_three_digits.append([cleaned_numbers[i]])
            is_even = False  # Hay un número sin par
    return pairs_of_three_digits, is_even, all_three_digits



