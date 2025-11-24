# ====================================================================
# BMC Blog - دليل البدء السريع (Quick Start Guide)
# ====================================================================

## 📋 الملفات المُنشأة

لديك الآن 4 ملفات رئيسية:

1. **VSCode_Prompts.md** - Prompts لـ GitHub Copilot في VS Code (10 خطوات)
2. **02-Sitecore-PowerShell-Scripts.ps1** - السكريبتات الأصلية (7 سكريبتات)
3. **03-Fixed-Sitecore-Scripts.ps1** - السكريبتات المحسّنة (6 سكريبتات) ⭐ **استخدم هذا**
4. **00-Execution-Plan.md** - الخطة الكاملة

---

## 🚀 كيف تبدأ؟

### **الطريقة الموصى بها:**

#### 1️⃣ ابدأ بـ Sitecore أولاً (أسرع)

```
1. افتح Sitecore PowerShell ISE
2. افتح ملف: 03-Fixed-Sitecore-Scripts.ps1
3. شغّل السكريبتات بالترتيب (SCRIPT 1 → SCRIPT 6)
4. في النهاية سيكون كل شيء جاهز في Sitecore
```

**لماذا هذه الطريقة؟**
- لأن Sitecore items يمكن إنشاؤها بسرعة
- لا تحتاج كود C# معقد
- يمكنك رؤية النتائج فوراً
- إذا حصل خطأ، يمكنك إعادة المحاولة بسهولة

#### 2️⃣ ثم استخدم VS Code (إذا احتجت تخصيص)

```
1. افتح VS Code مع GitHub Copilot
2. افتح ملف: VSCode_Prompts.md
3. استخدم الـ Prompts إذا أردت إضافة features جديدة
```

**متى تحتاج VS Code Prompts؟**
- إذا أردت إضافة Controllers جديدة
- إذا أردت إنشاء Custom logic
- إذا أردت Build automation scripts

---

## 📝 الخطوات بالتفصيل (Sitecore فقط)

### **SCRIPT 1: إنشاء Layout** (5 دقائق)
```powershell
# انسخ SCRIPT 1 من 03-Fixed-Sitecore-Scripts.ps1
# الصق في PowerShell ISE
# اضغط F5

# النتيجة المتوقعة:
[✓] BMC folder created
[✓] Blog Layout created successfully!
Layout ID: {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
```

**ماذا يفعل؟**
- ينشئ مجلد BMC في /sitecore/layout/Layouts
- ينشئ Blog Layout item
- يربطه بـ BlogLayout.cshtml

---

### **SCRIPT 2: إنشاء Renderings** (10 دقائق)
```powershell
# انسخ SCRIPT 2
# الصق في PowerShell ISE
# اضغط F5

# النتيجة المتوقعة:
[✓] Header created
[✓] Footer created
[✓] Breadcrumb created
[✓] Newsletter Subscribe created
```

**ماذا يفعل؟**
- ينشئ 4 Controller Renderings
- يربطهم بالـ Controllers الصحيحة (BmcNavigation, BmcNewsletter)

---

### **SCRIPT 3: ربط Layout بالصفحات** (10-20 دقيقة)
```powershell
# انسخ SCRIPT 3
# الصق في PowerShell ISE
# اضغط F5

# السكريبت سيسألك:
Which pages do you want to assign layout to?
1. Home page only
2. Home + Direct children (Blog, etc.)
3. All pages (393 items - RECOMMENDED)  ← اختر هذا
4. Custom path

Enter your choice (1-4): 3
```

**النتيجة المتوقعة:**
```
Success: 350
Failed: 0
Skipped: 43 (folders وغيرها)
```

**ماذا يفعل؟**
- يربط Blog Layout بكل الصفحات
- يتخطى الـ Folders والعناصر غير المطلوبة تلقائياً

---

### **SCRIPT 4: إضافة Renderings** (15-25 دقيقة)
```powershell
# انسخ SCRIPT 4
# الصق في PowerShell ISE
# اضغط F5

# السكريبت سيسألك:
Do you want to add renderings to:
1. Pages that have layout already  ← اختر هذا
2. Specific page

Enter your choice (1-2): 1
```

**النتيجة المتوقعة:**
```
Total Pages: 350
Total Rendering Operations: 1050 (350 × 3 renderings)
Success: 1050
Skipped: 0
Failed: 0
```

**ماذا يفعل؟**
- يضيف Header لكل صفحة
- يضيف Breadcrumb لكل صفحة
- يضيف Footer لكل صفحة

---

### **SCRIPT 5: النشر** (5-10 دقائق)
```powershell
# انسخ SCRIPT 5
# الصق في PowerShell ISE
# اضغط F5

# السكريبت سيسألك:
Do you want to publish all changes now? (y/n): y
```

**النتيجة المتوقعة:**
```
[✓] Layout published
[✓] Renderings published
[✓] Content published
```

**ماذا يفعل؟**
- ينشر كل التغييرات إلى Web database
- يجعل الموقع جاهز للمشاهدة

---

### **SCRIPT 6: التحقق النهائي** (دقيقتين)
```powershell
# انسخ SCRIPT 6
# الصق في PowerShell ISE
# اضغط F5

# النتيجة المتوقعة:
✓ SETUP COMPLETED SUCCESSFULLY!

Next Steps:
1. Test your site: http://abdo.sc/home
2. Check Experience Editor
3. Verify renderings are showing
```

**ماذا يفعل؟**
- يتحقق من وجود Layout
- يتحقق من وجود Renderings
- يتحقق من الصفحات
- يعطيك تقرير نهائي

---

## ⚠️ الأخطاء الشائعة وحلولها

### خطأ: "Layout folder not found"
**السبب:** مشكلة في Sitecore
**الحل:** 
```powershell
# تأكد من أنك في PowerShell ISE داخل Sitecore
# ليس PowerShell العادي في Windows
```

### خطأ: "Cannot edit item"
**السبب:** العنصر محجوز من قبل مستخدم آخر
**الحل:**
```
1. Content Editor
2. اضغط Right-click على العنصر
3. اختر Unlock
```

### خطأ: "Device not found"
**السبب:** مشكلة في Sitecore installation
**الحل:**
```
تأكد من أن Sitecore يعمل بشكل صحيح
/sitecore/layout/Devices/Default يجب أن يكون موجود
```

### خطأ: "Out of memory"
**السبب:** محاولة معالجة 393 صفحة دفعة واحدة
**الحل:**
```
1. في SCRIPT 3، اختر Option 4 (Custom path)
2. اعمل على دفعات صغيرة:
   - أولاً: /sitecore/content/BMC/BmcBlog/Home/Blog
   - ثانياً: /sitecore/content/BMC/BmcBlog/Home
   - ثالثاً: باقي الصفحات
```

---

## 📊 Timeline المتوقع

| المرحلة | الوقت | الملاحظات |
|---------|-------|-----------|
| SCRIPT 1 | 5 دقائق | سريع |
| SCRIPT 2 | 10 دقائق | سريع |
| SCRIPT 3 | 15 دقيقة | قد يطول إذا كان عدد الصفحات كبير |
| SCRIPT 4 | 20 دقيقة | الأطول - 1050 عملية |
| SCRIPT 5 | 10 دقائق | يعتمد على حجم Content |
| SCRIPT 6 | 2 دقيقة | سريع |
| **المجموع** | **~60 دقيقة** | ساعة واحدة تقريباً |

---

## ✅ Checklist النهائي

بعد إنهاء كل السكريبتات، تأكد من:

- [ ] Layout item موجود في Sitecore
- [ ] 4 Rendering items موجودة
- [ ] Home page له Layout
- [ ] Home page له 3 Renderings (Header, Breadcrumb, Footer)
- [ ] Blog page له Layout
- [ ] جميع Blog Posts لها Layout
- [ ] كل شيء منشور على Web database
- [ ] http://abdo.sc/home يفتح بدون أخطاء
- [ ] Header يظهر في الموقع
- [ ] Footer يظهر في الموقع

---

## 🎯 الخطوات التالية (بعد الانتهاء)

1. **اختبر الموقع:**
   ```
   http://abdo.sc/home
   http://abdo.sc/blog
   ```

2. **افتح Experience Editor:**
   ```
   Content Editor → Home page → Experience Editor
   ```

3. **إذا كان كل شيء يعمل:**
   - ابدأ في إضافة Content
   - أضف صور للـ Blog Posts
   - أضف Categories و Tags

4. **إذا كان هناك مشاكل:**
   - افحص Sitecore logs: C:\inetpub\wwwroot\abdo.sc\App_Data\logs
   - شغّل SCRIPT 6 للتحقق
   - اعمل Recycle للـ App Pool

---

## 💡 نصائح مهمة

### 1. احفظ كل تقدم
```powershell
# بعد كل SCRIPT ناجح:
# لا تغلق PowerShell ISE فوراً
# تأكد من النتائج أولاً
```

### 2. استخدم Diagnostic قبل وبعد
```powershell
# قبل:
.\01-Check-SitecoreStructure.ps1

# بعد SCRIPT 6:
.\01-Check-SitecoreStructure.ps1

# قارن النتائج
```

### 3. لا تستعجل
```
كل SCRIPT يأخذ وقت
خصوصاً SCRIPT 3 و 4
لا تقاطعهم أثناء التشغيل
```

### 4. Backup أولاً
```
Control Panel → Backup & Restore
Create Backup قبل البدء
```

---

## 📞 إذا واجهت مشكلة

1. **اقرأ رسالة الخطأ بعناية**
2. **ابحث في قسم "الأخطاء الشائعة" أعلاه**
3. **شغّل SCRIPT 6 للتحقق من الوضع الحالي**
4. **افحص Sitecore logs**

---

## 🎉 بالتوفيق!

هذا كل شيء! السكريبتات مُجهَّزة وجاهزة.

**ابدأ الآن:**
1. افتح Sitecore PowerShell ISE
2. افتح ملف 03-Fixed-Sitecore-Scripts.ps1
3. انسخ SCRIPT 1
4. اضغط F5

**في أقل من ساعة، سيكون موقعك جاهز! 🚀**
