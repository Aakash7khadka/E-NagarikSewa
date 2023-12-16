import cv2
import numpy as np
import tensorflow as tf
import datetime
import time
from gtts import gTTS
import playsound
import pymssql
import threading
import os
from PIL import Image

name_map={'aakash':"AAKASH777KHADKA@GMAIL.COM",'sudeep':"SPKHAREL92@GMAIL.COM",'vishal':'BISHALKAN6A@GMAIL.COM'}
# gpus = tf.config.experimental.list_physical_devices('GPU')
# if gpus:
#   try:
#     for gpu in gpus:
#       tf.config.experimental.set_memory_growth(gpu, True)
#   except RuntimeError as e:
#     print(e)
from tensorflow import keras
from tensorflow.keras.applications import DenseNet201
from tensorflow.keras.applications.densenet import preprocess_input
from tensorflow.keras.preprocessing.image import img_to_array,load_img
# from tensorflow.keras.applications.densenet import preprocess_input

model=tf.keras.models.load_model(
    'final',
    custom_objects=None, compile=True)

face_classifier=cv2.CascadeClassifier('haarcascade_frontalface_default.xml')
cap=cv2.VideoCapture(0)


# def crop_face(img):
#   faces=face_classifier.detectMultiScale(img,1.3,5)
#   if str(faces) =="()":
#     return None

#   for(x,y,w,h) in faces:
#     return img[y-10:y+h+50,x-10:x+w+50]
five_minutes_start=0
start_time=0
end_time=0
names=['aakash','sudeep','vishal']
count={'aakash':0,'sudeep':0,'vishal':0}



def create_equalized_image(image):
    flat_image=image.flatten()
    histogram=create_histogram(flat_image)

    N=sum(histogram)
    # #normalizing the histogram
    n_hist=histogram/N

    cs=cumsum(n_hist)


    cs*=255

    cs=np.round(cs)
    cs=cs.astype(int)

    img_new=cs[flat_image]
    img_new=img_new.astype(np.uint8)

    img_new=np.reshape(img_new,image.shape)
    return img_new





def create_histogram(image):
    histogram=np.zeros(256)
    #loop through all pixel
    for pixel in image:
        histogram[pixel]+=1
    return histogram


#calculate cumilitive sum
def cumsum(hist):
    cs=[]
    for i in range(len(hist)):
        cs.append(sum(hist[:i]))
    return np.array(cs)


# Load functions
def face_extractor(img):
    # Function detects faces and returns the cropped face
    # If no face detected, it returns the input image
    
    #gray = cv2.cvtColor(img,cv2.COLOR_BGR2GRAY)
    faces = face_classifier.detectMultiScale(img, 1.3, 15)
    
    if faces == ():
        return None
    
    # Crop all faces found
    for (x,y,w,h) in faces:
        x=x-10
        y=y-10
        cropped_face = img[y:y+h+20, x:x+w]

    return cropped_face


def face_counter(name):
  global start_time
  global count
  count[name]=count[name]+1
  print(count)
  if(start_time==0):
    start_time=time.time()
  
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


while True:
  # if(datetime.datetime.now().hour==6 and datetime.datetime.now().minute==0):
  # minute_now=datetime.datetime.now().minute
  # second_now=datetime.datetime.now().second

  # if( (minute_now%5==0 or minute_now==0)and second_now==0):
  #   for name in names:
  #     count[name]=0

  if(five_minutes_start==0):
    five_minutes_start=time.time()
  if((time.time()-five_minutes_start)>360):
    five_minutes_start=0
    for name in name_map.keys():
      count[name]=0


  ret,frame=cap.read()
  
  # frame=cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
  faces=face_classifier.detectMultiScale(frame,1.3,5)
  if str(faces) !="()":
    face=frame
    for(x,y,w,h) in faces:
      face=frame[y:y+h,x:x+w]
      try:
        face=cv2.resize(face,(224,224))
      except:
        continue
      

      # img = img_to_array(face)
      
      # input=np.array([img])
      # input=np.squeeze(input,axis=0)
      # print(input.shape)
      input=cv2.cvtColor(face, cv2.COLOR_BGR2GRAY)
      # print(input.shape)
      input=create_equalized_image(input)
      
      input=np.repeat(input[..., np.newaxis], 3, -1)
      # print(input.shape)
      input=np.expand_dims(input,axis=0)
      # print(input.shape)
      input=preprocess_input(input)
    # print(type(img))
    # cv2.imshow('img',face)
           
     
      # input=np.squeeze(input,axis=0)
      # print(input.shape)
      pred = model.predict(input)
      print(pred)
      # print(np.argmax(pred))
      lab=np.argmax(pred)
      cv2.rectangle(frame,(x,y),(x+w,y+h),(0,255,0),2)
    
                     
    name="None matching"
        
    if(pred[0][0]>0.98):
      name='aakash'
      print(count)
      # face_counter(name)
      # if(start_time==0):
      #   start_time=time.time()
      
      # if((time.time()-start_time)>10):
      #   if(count[name]>10):
      #     print('aakash done')
      #   else:
      #     count[name]=0
      #     start_time=0


    elif(pred[0][1]>0.98):
      name='sudeep'
      # face_counter(name)
    elif(pred[0][2]>0.98):
      name='vishal'
      # face_counter(name)
    else:
      name="None matching"
    if (name!="None matching"):
      face_counter(name)
    cv2.putText(frame,name, (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)
  cv2.imshow('img',frame)
    # else:
    #     cv2.putText(frame,"No face found", (50, 50), cv2.FONT_HERSHEY_COMPLEX, 1, (0,255,0), 2)
    # cv2.imshow('Video', frame)
  
  if cv2.waitKey(1)==ord('q'):
    break