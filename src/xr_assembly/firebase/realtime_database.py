from __future__ import absolute_import
from __future__ import division
from __future__ import print_function


import pyrebase
import os


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

# reference to firebase database
db = firebase.database()

def stream_handler(message):
    print(message["event"])
    print(message["path"])
    print(message["data"])

# get keys_built
def get_keys_built():
    keys_built = []
    keys = db.child("Built Keys").get()
    if keys.each():
        for key in keys.each():
            #print("key to built key ", key.key())
            keys_built.append(key.val())
    return keys_built

# get users' ids
def get_users():
    users_ids = []
    users = db.child("Users").get()
    for user in users.each():
        print(user.key())
        users_ids.append(user.key())
        #print("Selected Key is ", user.val()["selectedKey"])
        #print("Selected by user Nr.", user.val()["userID"])
    return users_ids

# set keys build.
# note:erases previously saved data
def set_keys_built(keys):
    data = {}
    for key in keys:
        data[str(key)] = str(key)
    db.child("Built Keys").set(data)

def remove_key_built(key):
    db.child("Built Keys").child(str(key)).remove()

# update data
def add_key_built(new_key_built):
    db.child("Built Keys").update({str(new_key_built):str(new_key_built)})

# listen to changes
def listen():
    my_stream = db.child("Built Keys").stream(stream_handler)

# add a stream_id for multiple streams
#my_stream = db.child("posts").stream(stream_handler, stream_id="new_post")

def close_stream(my_stream):
    my_stream.close()

if __name__ == "__main__":

    
    #add_key_built(4)
    remove_key_built(10)
    print(get_keys_built())
    #my_stream = db.child("Built Keys").stream(stream_handler)
    #my_stream = db.child("Users").stream(stream_handler)
    #close_stream(my_stream)
    #remove_key_built(17)
    

