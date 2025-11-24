# ====================================================================
# BMC Blog - خطة التنفيذ الكاملة (Step-by-Step Execution Plan)
# ====================================================================

## نظرة عامة

هذا الدليل يوضح الخطوات الكاملة لإعداد BMC Blog من الصفر حتى يعمل بشكل كامل.
سنستخدم كل من Visual Studio Code و Sitecore PowerShell ISE.

---

## 📋 المتطلبات الأساسية

قبل البدء، تأكد من:
- ✅ Visual Studio 2019 مثبت
- ✅ GitHub Copilot مفعل في VS Code
- ✅ Sitecore 9.3 يعمل على http://abdo.sc
- ✅ Sitecore PowerShell Extensions (SPE) مثبت
- ✅ لديك صلاحيات Administrator

---

## 🎯 الخطة (Phase-by-Phase)

### **PHASE 1: إعداد الكود (Visual Studio Code)**

#### المدة المتوقعة: 2-3 ساعات

**الخطوة 1.1: تحضير البيئة**
```
1. افتح المشروع في VS Code
2. تأكد من أن GitHub Copilot يعمل
3. افتح Terminal في VS Code
```

**الخطوة 1.2: إنشاء Helper Classes (استخدم VSCode_Prompts.md)**

نفذ الـ Prompts بالترتيب:

| الترتيب | الملف | Prompt | الوقت المتوقع |
|---------|-------|--------|----------------|
| 1 | LayoutItemManager.cs | STEP 1 | 15 دقيقة |
| 2 | RenderingItemManager.cs | STEP 2 | 15 دقيقة |
| 3 | PresentationManager.cs | STEP 3 | 20 دقيقة |
| 4 | TemplateManager.cs | STEP 4 | 20 دقيقة |
| 5 | SitecoreInitializer.cs | STEP 5 | 20 دقيقة |
| 6 | InitializeBlogProcessor.cs | STEP 6 | 15 دقيقة |
| 7 | ItemExtensions.cs | STEP 7 | 15 دقيقة |
| 8 | Controllers Review | STEP 8 | 20 دقيقة |
| 9 | Build-And-Deploy.ps1 | STEP 9 | 10 دقيقة |
| 10 | Test-SitecoreSetup.ps1 | STEP 10 | 10 دقيقة |

**كيفية استخدام كل Prompt:**

```
1. افتح GitHub Copilot Chat في VS Code (Ctrl+Shift+I)
2. انسخ الـ Prompt من ملف VSCode_Prompts.md
3. الصق الـ Prompt في Copilot Chat
4. راجع الكود المُنشأ
5. احفظ الملف في المكان الصحيح
6. كرر للـ Prompt التالي
```

**الخطوة 1.3: Build & Review**

```powershell
# في Visual Studio
1. افتح Solution في Visual Studio 2019
2. Build > Rebuild Solution
3. تأكد من عدم وجود Errors
4. راجع الـ Warnings (يمكن تجاهل معظمها)
```

**نقطة تفتيش (Checkpoint 1):**
- ✅ جميع الملفات الـ 10 تم إنشاؤها
- ✅ Solution تم Build بنجاح
- ✅ لا توجد أخطاء حرجة

---

### **PHASE 2: Deploy إلى Sitecore**

#### المدة المتوقعة: 30 دقيقة

**الخطوة 2.1: Deploy DLLs**

```powershell
# شغل Build-And-Deploy.ps1 من PowerShell
cd C:\Projects\BmcHelix\BmcBlog
.\scripts\Build-And-Deploy.ps1
```

**ماذا يفعل السكريبت:**
- ينسخ DLLs إلى C:\inetpub\wwwroot\abdo.sc\bin\
- ينسخ Views إلى C:\inetpub\wwwroot\abdo.sc\Views\
- ينسخ Config files

**الخطوة 2.2: Recycle App Pool**

```
1. افتح IIS Manager
2. اذهب إلى Application Pools
3. اضغط Right-click على pool الخاص بـ abdo.sc
4. اختر Recycle
```

**الخطوة 2.3: Test Deployment**

```
1. افتح المتصفح
2. اذهب إلى http://abdo.sc/?initBlog=true
3. يجب أن تظهر رسالة "Initialization Started" أو شيء مشابه
```

**نقطة تفتيش (Checkpoint 2):**
- ✅ DLLs موجودة في bin folder
- ✅ Views موجودة في Views folder
- ✅ Sitecore يعمل بدون أخطاء

---

### **PHASE 3: إعداد Sitecore Items (PowerShell)**

#### المدة المتوقعة: 1-2 ساعة

**الخطوة 3.1: افتح Sitecore PowerShell ISE**

```
1. افتح http://abdo.sc/sitecore
2. سجل دخول كـ Admin
3. Start Menu → PowerShell ISE
```

**الخطوة 3.2: نفذ السكريبتات بالترتيب**

استخدم ملف `02-Sitecore-PowerShell-Scripts.ps1`

| الترتيب | السكريبت | الغرض | الوقت |
|---------|----------|-------|-------|
| 1 | SCRIPT 1 | إنشاء Layout Item | 5 دقائق |
| 2 | SCRIPT 2 | إنشاء Rendering Items | 10 دقائق |
| 3 | SCRIPT 3 | ربط Layout بـ Home | 5 دقائق |
| 4 | SCRIPT 4 | إضافة Renderings لـ Home | 10 دقائق |
| 5 | SCRIPT 5 | نسخ Presentation لصفحات أخرى | 15 دقيقة |
| 6 | SCRIPT 6 | نشر التغييرات | 10 دقيقة |
| 7 | SCRIPT 7 | التحقق النهائي | 5 دقائق |

**كيفية تشغيل كل سكريبت:**

```
1. افتح ملف 02-Sitecore-PowerShell-Scripts.ps1
2. ابحث عن SCRIPT X (حسب الترتيب)
3. انسخ الكود من بداية SCRIPT X حتى نهايته
4. الصق في PowerShell ISE
5. اضغط Execute (F5)
6. انتظر النتيجة
7. اقرأ الرسائل (Success/Warning/Error)
8. انتقل للسكريبت التالي
```

**الخطوة 3.3: معالجة الأخطاء (إن وجدت)**

**إذا ظهر خطأ في SCRIPT 1:**
```
المشكلة: Layout folder not found
الحل: تأكد من المسار /sitecore/layout/Layouts موجود
```

**إذا ظهر خطأ في SCRIPT 3:**
```
المشكلة: Cannot assign layout
الحل: تأكد من أن Layout item موجود من SCRIPT 1
```

**إذا ظهر خطأ في SCRIPT 4:**
```
المشكلة: Rendering not found
الحل: تأكد من تشغيل SCRIPT 2 بنجاح
```

**نقطة تفتيش (Checkpoint 3):**
- ✅ Layout item موجود في Sitecore
- ✅ Rendering items موجودة
- ✅ Home page لها Layout
- ✅ Home page لها Renderings
- ✅ جميع Items منشورة على Web database

---

### **PHASE 4: الاختبار النهائي**

#### المدة المتوقعة: 30 دقيقة

**الخطوة 4.1: اختبار من Content Editor**

```
1. افتح Content Editor
2. اذهب إلى /sitecore/content/BMC/BmcBlog/Home
3. اضغط على تبويب "Presentation"
4. تحقق من وجود:
   - Layout: Blog Layout
   - Renderings: Header, Footer, Breadcrumb
```

**الخطوة 4.2: اختبار من Experience Editor**

```
1. اضغط Right-click على Home item
2. اختر "Experience Editor"
3. يجب أن يفتح الموقع بدون أخطاء
4. تحقق من ظهور:
   - Header
   - Footer
   - Breadcrumb
```

**الخطوة 4.3: اختبار من المتصفح**

```
1. افتح متصفح جديد (Incognito Mode)
2. اذهب إلى http://abdo.sc/home
3. افحص الصفحة:
   - يجب ألا تظهر أخطاء ASP.NET
   - يجب أن يظهر Header
   - يجب أن يظهر Footer
   - يجب أن تظهر Bootstrap بشكل صحيح
```

**الخطوة 4.4: شغل Verification Script**

```powershell
# في Sitecore PowerShell ISE
# شغل SCRIPT 7 من ملف 02-Sitecore-PowerShell-Scripts.ps1
```

**النتيجة المتوقعة:**
```
[✓] All checks passed!
Your blog is ready to use!
```

**نقطة تفتيش (Checkpoint 4 - النهائي):**
- ✅ الموقع يفتح بدون أخطاء
- ✅ Header و Footer يظهران
- ✅ Breadcrumb يعمل
- ✅ Bootstrap CSS يعمل
- ✅ لا توجد 404 errors للـ CSS/JS

---

## 🐛 استكشاف الأخطاء الشائعة

### خطأ: "The file cannot be requested directly"

**السبب:** RenderSection أو RenderBody موجود في Layout

**الحل:**
```
1. افتح BlogLayout.cshtml
2. تأكد من استبدال:
   - @RenderSection → @Html.Sitecore().Placeholder()
   - @RenderBody() → @Html.Sitecore().Placeholder("main")
3. احفظ واعمل Deploy
```

### خطأ: "Multiple types were found that match the controller"

**السبب:** تضارب في أسماء الكنترولر

**الحل:**
```
تأكد من أن الكنترولرز لها أسماء فريدة:
- BmcNavigationController (وليس NavigationController)
- BmcNewsletterController (وليس NewsletterController)
```

### خطأ: "View not found"

**السبب:** مجلد الـ Views لم يتم تحديثه

**الحل:**
```
1. تأكد من أن المجلدات بالأسماء الصحيحة:
   - Views/BmcNavigation/ (وليس Navigation)
   - Views/BmcNewsletter/ (وليس Newsletter)
2. اعمل Deploy مرة أخرى
```

### خطأ: "Layout not found in Sitecore"

**السبب:** Layout item لم يتم إنشاؤه

**الحل:**
```
1. شغل SCRIPT 1 من PowerShell
2. تحقق من Content Editor أن Layout موجود
3. إذا كان موجود، تأكد من Path صحيح
```

---

## 📝 Checklist النهائي

قبل أن تعتبر المشروع جاهز، تأكد من:

### في Visual Studio:
- [ ] جميع الملفات الـ 10 موجودة
- [ ] Solution يعمل Build بنجاح
- [ ] لا توجد أخطاء حرجة
- [ ] DLLs تم نسخها لـ bin folder

### في Sitecore Content Editor:
- [ ] Layout item موجود في /sitecore/layout/Layouts/BMC/
- [ ] Rendering items موجودة في /sitecore/layout/Renderings/BMC/
- [ ] Home page له Presentation Details
- [ ] Blog page له Presentation Details

### في Sitecore Web Database:
- [ ] Layout منشور
- [ ] Renderings منشورة
- [ ] Home page منشورة
- [ ] Blog pages منشورة

### في المتصفح:
- [ ] http://abdo.sc/home يفتح بدون أخطاء
- [ ] Header يظهر
- [ ] Footer يظهر
- [ ] Breadcrumb يعمل
- [ ] Bootstrap CSS يعمل
- [ ] لا توجد Console errors في Developer Tools

---

## 📊 جدول زمني متوقع

| المرحلة | الوقت المتوقع | الوقت الفعلي |
|---------|----------------|---------------|
| Phase 1: إعداد الكود | 2-3 ساعات | _______ |
| Phase 2: Deploy | 30 دقيقة | _______ |
| Phase 3: Sitecore Setup | 1-2 ساعة | _______ |
| Phase 4: Testing | 30 دقيقة | _______ |
| **المجموع** | **4-6 ساعات** | _______ |

---

## 🎓 ملاحظات مهمة

1. **احفظ كل تقدم في Git:**
   ```bash
   git add .
   git commit -m "Phase X completed"
   git push
   ```

2. **اعمل Backup للـ Sitecore قبل Phase 3:**
   - من Sitecore Control Panel
   - Backup & Restore
   - Create Backup

3. **إذا واجهت مشكلة:**
   - لا تستمر للمرحلة التالية
   - حل المشكلة أولاً
   - تحقق من Checkpoints

4. **استخدم Diagnostic Script:**
   ```powershell
   # شغله في أي وقت للتحقق من الوضع الحالي
   .\01-Check-SitecoreStructure.ps1
   ```

---

## 🚀 بعد الانتهاء

عندما تنجح في جميع الـ Checkpoints:

1. **استمتع بموقعك الجديد!** 🎉
2. **ابدأ في إضافة محتوى**
3. **اختبر المزيد من الصفحات**
4. **اعمل Performance testing**

---

**تم إعداد هذا الدليل بواسطة:** Claude AI
**تاريخ:** 2024-11-24
**المشروع:** BMC Blog - Sitecore 9.3
**الهيكل:** Helix Architecture

**ملاحظة:** هذا دليل شامل لكنه قد يحتاج تعديلات بسيطة حسب بيئتك الخاصة.
