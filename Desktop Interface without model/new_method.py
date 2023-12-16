import face_recognition
import cv2
import tensorflow as tf
import numpy as np
from gtts import gTTS
import playsound
import datetime
import os
import time
# from gtts import gTTS
# import playsound
import pymssql
import threading

model=tf.keras.models.load_model(
    'final',
    custom_objects=None, compile=True)
# import os
# from PIL import Image

# This is a demo of running face recognition on live video from your webcam. It's a little more complicated than the
# other example, but it includes some basic performance tweaks to make things run a lot faster:
#   1. Process each video frame at 1/4 resolution (though still display it at full resolution)
#   2. Only detect faces in every other frame of video.

# PLEASE NOTE: This example requires OpenCV (the `cv2` library) to be installed only to read from your webcam.
# OpenCV is *not* required to use the face_recognition library. It's only required if you want to run this
# specific demo. If you have trouble installing it, try any of the other demos that don't require it instead.

# Get a reference to webcam #0 (the default one)
video_capture = cv2.VideoCapture(0)
name_map={'aakash':"AAKASH777KHADKA@GMAIL.COM",'sudeep':"SPKHAREL92@GMAIL.COM",'vishal':'BISHALKAN6A@GMAIL.COM'}
def data_entry(name):
    conn = pymssql.connect(
    host=r'SQL5107.site4now.net',
    user=r'db_a84248_admin_admin',
    password=r'Admin@123',
    database='db_a84248_admin')
    cursor = conn.cursor()
    entry_time_sound(name)
    # date_now="'"+str(datetime.datetime.now().strftime("%Y-%m-%d"))+"'"
    # time_now="'"+str(datetime.datetime.now().strftime('%H:%M'))+"'"
    date_time="'"+str(datetime.datetime.today().strftime('%Y/%m/%d %I:%M:%S %p'))+"'"
    # query="INSERT INTO db_a753f6_rams.dbo.Attendances VALUES ("+str(0)+","+date_now+","+time_now+")"
    email_entry="'"+str(name_map[name])+"'"
    query="INSERT INTO db_a84248_admin.dbo.Attendances (Entry_time,UserEmail) VALUES ("+date_time+","+email_entry+")"
    # query="INSERT INTO db_a761c7_rams.dbo.Attendances VALUES ("+date_time+","+str(name_map[name])+")"
    print(query)
    cursor.execute(query)
    conn.commit()


def entry_time_sound(name):
  # date_time="'"+str(datetime.datetime.today().strftime('%I %M %p'))+"'"
  # text="Hello!" +name+"your entry time is "+date_time
  text="Hello!" +name
  tts = gTTS(text=text, lang='en')

  filename = "abc.mp3"
  tts.save(filename)
  playsound.playsound(filename)
  os.remove(filename)

def face_counter(name):
  global start_time
  global count
  count[name]=count[name]+1

  if(start_time==0):
    start_time=time.time()
  print(count)
  if((time.time()-start_time)>10):
    if(count[name]>8):
      print(name+' done')
      
      
    else:
      count[name]=0
      start_time=0
  if(count[name]==9):
    t1=threading.Thread(target=data_entry,args=(name,)).start()
    # playsound.playsound('sound/'+name+'.mp3')
  return

# Load a sample picture and learn how to recognize it.
aakash_khadka = face_recognition.load_image_file("1.jpg")
aakash_khadka_face_encoding = face_recognition.face_encodings(aakash_khadka)[0]

# Load a second sample picture and learn how to recognize it.
sudeep_kharel = face_recognition.load_image_file("2.jpg")
sudeep_kharel_face_encoding = face_recognition.face_encodings(sudeep_kharel)[0]

vishal_purkoti = face_recognition.load_image_file("3.jpg")
vishal_purkoti_face_encoding = face_recognition.face_encodings(vishal_purkoti)[0]
# Create arrays of known face encodings and their names
known_face_encodings = [
    aakash_khadka_face_encoding,
    sudeep_kharel_face_encoding,
    vishal_purkoti_face_encoding

]


five_minutes_start=0
start_time=0
end_time=0

count={'aakash':0,'sudeep':0,'vishal':0}
known_face_names = [
    "aakash",
    "sudeep",
    "vishal"
]

# Initialize some variables
face_locations = []
face_encodings = []
face_names = []
process_this_frame = True

while True:
    if(five_minutes_start==0):
        five_minutes_start=time.time()
    if((time.time()-five_minutes_start)>360):
        five_minutes_start=0
        for name in name_map.keys():
            count[name]=0

    # Grab a single frame of video
    ret, frame = video_capture.read()

    # Resize frame of video to 1/4 size for faster face recognition processing
    small_frame = cv2.resize(frame, (0, 0), fx=0.25, fy=0.25)

    # Convert the image from BGR color (which OpenCV uses) to RGB color (which face_recognition uses)
    rgb_small_frame = small_frame[:, :, ::-1]

    # Only process every other frame of video to save time
    if process_this_frame:
        # Find all the faces and face encodings in the current frame of video
        face_locations = face_recognition.face_locations(rgb_small_frame)
        face_encodings = face_recognition.face_encodings(rgb_small_frame, face_locations)

        face_names = []
        for face_encoding in face_encodings:
            # See if the face is a match for the known face(s)
            matches = face_recognition.compare_faces(known_face_encodings, face_encoding)
            name = "Not A Employee!"

            # # If a match was found in known_face_encodings, just use the first one.
            # if True in matches:
            #     first_match_index = matches.index(True)
            #     name = known_face_names[first_match_index]
            #     print(name)

            # Or instead, use the known face with the smallest distance to the new face
            # print(matches)
            face_distances = face_recognition.face_distance(known_face_encodings, face_encoding)
         
            best_match_index = np.argmin(face_distances)
    
            
            if matches[best_match_index]:
                name = known_face_names[best_match_index]
                if(name!="Unknown"):
                     face_counter(name)

            face_names.append(name)

    process_this_frame = not process_this_frame
   

    # Display the results
    for (top, right, bottom, left), name in zip(face_locations, face_names):
        # Scale back up face locations since the frame we detected in was scaled to 1/4 size
        top *= 4
        right *= 4
        bottom *= 4
        left *= 4

        # Draw a box around the face
        cv2.rectangle(frame, (left, top), (right, bottom), (0, 255, 0), 2)
        # cv2.rectangle(frame,(x,y),(x+w,y+h),(0,255,0),2)

        # Draw a label with a name below the face
        # cv2.rectangle(frame, (left, bottom - 35), (right, bottom), (0, 0, 255), cv2.FILLED)
        cv2.putText(frame,name, (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)
        font = cv2.FONT_HERSHEY_DUPLEX
        # cv2.putText(frame, name, (left + 6, bottom - 6), font, 1.0, (255, 255, 255), 1)
        cv2.putText(frame,name, (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)

    # Display the resulting image
    cv2.imshow('Video', frame)
    
    # Hit 'q' on the keyboard to quit!
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release handle to the webcam
video_capture.release()
cv2.destroyAllWindows()