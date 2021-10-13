import os
import pyrebase

config = {
    "apiKey": "AIzaSyDoBCTWViNEOcXgY1Ksk4wfw-bEjieCDhQ",
    "authDomain": "ar-fabrication.firebaseapp.com",
    "databaseURL": "https://ar-fabrication-default-rtdb.europe-west1.firebasedatabase.app",
    "projectId": "ar-fabrication",
    "storageBucket": "ar-fabrication.appspot.com",
    "messagingSenderId": "1047794230046",
    "appId": "1:1047794230046:web:32d5a461e8382706349f0f",
    "measurementId": "G-K8ZDJ560G4"
}

path_on_cloud = "stateRecord2.txt" # path to download/upload from/to cloud
path_local = "c:/Users/lidet/workspace/projects/xr_assembly/data/test.txt" # local path to store files

# initialize the connection to firebase
firebase = pyrebase.initialize_app(config)

# reference to firebase storage
storage = firebase.storage()

def upload_to_firebase(path_on_cloud, path_local):
    storage.child(path_on_cloud).put(path_local)

def download_from_firebase(path_on_cloud, path_local):
    storage.child(path_on_cloud).download(path_local)


if __name__ == "__main__":

    #download_from_firebase(path_on_cloud, path_local)
    upload_to_firebase(path_on_cloud, path_local)
