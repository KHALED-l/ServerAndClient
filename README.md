# 📦 Multi-threaded Compression Project
### IT342 - Network Programming | IT Department

---

## 📋 وصف المشروع

نظام Client-Server لضغط الملفات عبر الشبكة، يتكون من:
- **Server**: سيرفر متعدد الخيوط يستقبل الملفات ويضغطها ويعيد إرسالها
- **Client**: تطبيق بواجهة رسومية لاختيار الملف وإرساله واستقبال النسخة المضغوطة

---

## 🗂️ ملفات المشروع

| الملف | الوظيفة |
|---|---|
| `Program.cs` | نقطة البداية - تختار Server أم Client |
| `CompressionServer.cs` | منطق السيرفر (Multi-threading + GZip) |
| `CompressionClient.cs` | منطق العميل (إرسال واستقبال الملف) |
| `ServerForm.cs` | واجهة السيرفر الرسومية |
| `ClientForm.cs` | واجهة العميل الرسومية |

---

## ⚙️ متطلبات التشغيل

- **Visual Studio** 2019 أو أحدث
- **.NET Framework** 4.7.2 أو أحدث
- **Windows** 10 أو أحدث

---

## 🚀 طريقة إنشاء المشروع في Visual Studio

### الخطوة 1: إنشاء المشروع
```
File → New → Project
→ Windows Forms App (.NET Framework)
→ Name: CompressionProject
→ OK
```

### الخطوة 2: حذف Form1.cs
```
Solution Explorer → كليك يمين على Form1.cs → Delete → OK
```

### الخطوة 3: تعديل Program.cs
```
كليك يمين على Program.cs → View Code
→ احذف كل المحتوى
→ انسخ محتوى ملف Program.cs
```

### الخطوة 4: إضافة الملفات الأربعة
```
كليك يمين على اسم المشروع → Add → Class
→ سمّيه CompressionServer.cs → Add → انسخ المحتوى

كرر نفس الخطوة لـ:
→ CompressionClient.cs
→ ServerForm.cs
→ ClientForm.cs
```

### الخطوة 5: التشغيل
```
اضغط F5 أو زر ▶
```

---

## 📖 طريقة الاستخدام

### تشغيل السيرفر
1. شغّل البرنامج ← اختار **YES** (Server)
2. اكتب رقم الـ **Port** (الافتراضي: `9000`)
3. اضغط **▶ Start Server**
4. السيرفر جاهز لاستقبال الاتصالات ✅

### تشغيل العميل
1. شغّل البرنامج مرة ثانية ← اختار **NO** (Client)
2. اكتب **Server IP** (لو على نفس الجهاز: `127.0.0.1`)
3. اكتب نفس الـ **Port** اللي السيرفر شغّال عليه
4. اضغط **📂 Browse** واختار الملف
5. اضغط **🚀 إرسال وضغط**
6. انتظر حتى يكتمل الـ Progress Bar
7. الملف المضغوط `.gz` يُحفظ في نفس مجلد الملف الأصلي ✅

---

## 🔄 آلية العمل (Protocol)

```
Client                              Server
  |                                   |
  |──── fileSize     (8 bytes) ──────►|
  |──── nameLength   (4 bytes) ──────►|
  |──── fileName     (N bytes) ──────►|
  |──── fileData     (N bytes) ──────►|
  |                                   |  ← يضغط بـ GZip
  |◄─── compressedSize (8 bytes) ─────|
  |◄─── compressedData (N bytes) ─────|
  |                                   |
  يحفظ الملف .gz على الجهاز
```

---

## 🧵 Multi-threading

كل Client يتصل بالسيرفر يحصل على **Thread منفصل**:

```
السيرفر الرئيسي (Main Thread)
    │
    ├── Client 1 → Thread 1  (يضغط ملف A)
    ├── Client 2 → Thread 2  (يضغط ملف B)  ← في نفس الوقت
    └── Client 3 → Thread 3  (يضغط ملف C)
```

---

## 📊 خوارزمية الضغط

المشروع يستخدم **GZip** المدمج في .NET:
```csharp
using System.IO.Compression;

// ضغط
using (var gz = new GZipStream(output, CompressionMode.Compress))
    gz.Write(data, 0, data.Length);

// فك الضغط
using (var gz = new GZipStream(input, CompressionMode.Decompress))
    gz.CopyTo(output);
```

---

## 📁 هيكل Solution Explorer

```
📁 Solution 'CompressionProject'
   📁 CompressionProject
      📄 Program.cs
      📄 CompressionServer.cs
      📄 CompressionClient.cs
      📄 ServerForm.cs
      📄 ClientForm.cs
      📁 Properties
         📄 AssemblyInfo.cs
```

---

## ⚠️ ملاحظات مهمة

- شغّل السيرفر **أولاً** قبل العميل
- لو السيرفر والعميل على **نفس الجهاز** استخدم IP: `127.0.0.1`
- لو على **أجهزة مختلفة** استخدم IP الجهاز الحقيقي للسيرفر
- الـ Firewall ممكن يحتاج تسمح لـ Port `9000`
- الملف المضغوط بيتحفظ بامتداد **`.gz`** في نفس مجلد الملف الأصلي

---

## 👨‍💻 معلومات المشروع

| | |
|---|---|
| **المادة** | IT342 - Network Programming |
| **القسم** | IT Department |
| **اللغة** | C# |
| **Framework** | .NET Framework - Windows Forms |
| **بروتوكول الشبكة** | TCP/IP |
| **خوارزمية الضغط** | GZip (System.IO.Compression) |
