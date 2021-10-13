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

# initialize the connection to firebase
firebase = pyrebase.initialize_app(config)

# reference to firebase storage
storage = firebase.storage()

# path to download from
path_on_cloud = "stateRecord.txt"

# local path to store files
path_local = "c:/Users/lidet/workspace/projects/collective_assembly/data/states_from_cloud.txt"

#storage.child("Images/my_image.png").get_url()
storage.child(path_on_cloud).download(path_local)


