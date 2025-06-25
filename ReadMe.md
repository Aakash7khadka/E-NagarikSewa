
# E-Nagarik Sewa: A Face Recognition-Based Appointment System

> A final year BSc CSIT project submitted to Texas International College, Tribhuvan University.

---

## 📌 Project Overview

**E-Nagarik Sewa** is a smart e-governance solution developed to improve citizen interaction with local government bodies in Nepal. It allows users to book appointments with government employees based on real-time availability—verified via a **face recognition system**. This ensures efficient, transparent, and time-saving public service delivery.

---

## ✨ Features

- 🔍 Real-time face recognition to detect employee presence
- 📅 Online appointment booking system for citizens
- 📧 Email notifications for both employees and citizens
- 👤 Role-based access: Admin, Employee, Citizen
- 🌐 Responsive UI for desktop and mobile

---

## 🧠 Technologies Used

### Backend & Frameworks
- ASP.NET Core MVC (C#)
- Python (Face Recognition)
- MSSQL Server (Database)

### Machine Learning
- Convolutional Neural Network (CNN)
- Modified DenseNet-201 (pretrained on ImageNet)
- Histogram Equalization for image preprocessing
- Stochastic Gradient Descent (SGD) optimizer

### Frontend
- HTML5, CSS3, Bootstrap
- JavaScript

### Tools & Platforms
- Visual Studio
- Google Colab (Model Training)
- SendGrid (Email API)
- SmarterASP.NET (Web Hosting)
- Adobe XD, Draw.io, Whimsical (Design & Diagramming)

---

## 🧩 System Components

### 1. Face Recognition Module
- Built in Python
- Takes live webcam input to recognize employees
- Automatically updates availability status in the database
- Prevents duplicate recognition within 2 hours

### 2. Web Portal
- User registration and email verification
- Appointment booking with available employees
- Admin panel for role management
- Attendance tracking interface

---

## ⚙️ Setup Instructions

### 1. Clone the repository
```bash
git clone https://github.com/Aakash7khadka/E-NagarikSewa.git
```

### 2. Setup the Web Application
- Open the `.sln` file in Visual Studio
- Configure `appsettings.json` with your MSSQL DB connection
- Run migrations and seed initial data
- Build and run the app

### 3. Setup Face Recognition Module
- Install Python 3.8+
- Install dependencies:
```bash
pip install -r requirements.txt
```
- Train or load pre-trained model
- Run the face recognition script

---

## 📊 Model Summary

- **Architecture**: Modified DenseNet-201
- **Input Size**: 512×512 grayscale images
- **Training Accuracy**: ~82%
- **Test Accuracy**: ~99%
- **Optimizer**: SGD with 0.01 learning rate, momentum 0.7
- **Loss Function**: Categorical Cross-Entropy

---

## ✅ Testing

- ✔️ Unit Testing: Registration, Login, Face Recognition, Appointment Booking
- ✔️ System Testing: Notifications, Role Management, Availability
- ✅ All modules tested and integrated successfully

---

## 📌 Limitations

- Supports only *vital registration* service currently
- Attendance is shown but not logged long-term
- Available only in English
- Dependent on active webcam and internet for recognition

---

## 🚀 Future Improvements

- Add services like social security, tax payments, public notices
- Multilingual support (Nepali, English, others)
- Live chat between citizens and service providers
- Persistent attendance records and analytics

---

## 👥 Authors

- **Aakash Khadka**  
- **Sudeep Kharel**  
- **Vishal Purkuti**

---

## 📄 License

This project was developed as an academic submission and is intended for educational and non-commercial use only.

---

## 🏫 Affiliation

Department of Computer Science & IT  
Texas International College  
Tribhuvan University, Nepal  
