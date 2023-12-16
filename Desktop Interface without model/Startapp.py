#!/usr/bin/python
import imp
import sys
import sys
import os
from tkinter import*
from tkinter import messagebox

from matplotlib.pyplot import margins

root=Tk()
root.geometry("520x350")
root.title("Enagarik Face Recognition System")
# canvas=Canvas(root,height=700,width=600)
# canvas.pack()

def mainhelp():
    messagebox.showinfo("showinfo", "- Click on Run Camera.\n - Wait for the camera to open.\n - Restart the application if error occurs")


def about():
     messagebox.showinfo("showinfo", "Developed by Vishal, Sudeep and Aakash for BSc CSIT Final Year Project")

menubar = Menu(root)
helpmenu = Menu(menubar, tearoff=0)
helpmenu.add_command(label="Run Camera", command=mainhelp)
helpmenu.add_command(label="About...", command=about)
menubar.add_cascade(label="Help", menu=helpmenu)

header = Label(root, text="E-Nagarik Sewa Employee Face Recognition System",font=("Arial Bold", 15),fg='#fff',bg='#6466ca',pady=20,padx=10)
instruction = Label(root, text="Click on Run Camera to start the camera",height=5)

header.pack()
instruction.pack()


    
def helloCallBack():
    os.system('python new_method.py')
    messagebox.showinfo("showinfo", "Camera Closed")

    
B=Button(root,text="Run Camera",command= helloCallBack)
B.pack()
root.config(menu=menubar)
root.mainloop()