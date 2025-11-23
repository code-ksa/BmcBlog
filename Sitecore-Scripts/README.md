# دليل استخدام PowerShell Scripts لـ BMC Blog

## نظرة عامة

هذا المجلد يحتوي على PowerShell Scripts لإنشاء وتكوين بنية BMC Blog في Sitecore. يجب تشغيل هذه السكريبتات في **Sitecore PowerShell ISE**.

---

## 📋 قائمة السكريبتات

### 1. `Create-BMC-Blog-Templates.ps1`
**الوظيفة:** إنشاء جميع Templates المطلوبة للمدونة

**Templates المنشأة:**
- ✅ Site Template (`/sitecore/templates/Project/BMC/Site`)
- ✅ Blog Root Template (`/sitecore/templates/Feature/BMC/Blog/Blog Root`)
- ✅ Blog Post Template (`/sitecore/templates/Feature/BMC/Blog/Blog Post`)
- ✅ Category Template (`/sitecore/templates/Feature/BMC/Blog/Category`)
- ✅ Author Template (`/sitecore/templates/Feature/BMC/Blog/Author`)
- ✅ Blog Listing Template (`/sitecore/templates/Feature/BMC/Blog/Blog Listing`)

**الميزات:**
- ✅ التحقق من Templates الموجودة وعدم تكرارها
- ✅ إنشاء جميع Fields المطلوبة
- ✅ دعم اللغتين العربية والإنجليزية في Display Names

---

### 2. `Update-Template-IDs.ps1`
**الوظيفة:** استخراج Template IDs الفعلية من Sitecore وإنشاء ملف C# Constants

**المخرجات:**
- ✅ ملف `Templates.cs` محدّث بـ IDs الصحيحة
- ✅ يتم حفظه في: `$SitecoreDataFolder\temp\Templates.cs`

**الاستخدام:**
بعد إنشاء Templates، قم بتشغيل هذا السكريبت لاستخراج IDs ونسخها إلى المشروع.

---

### 3. `Create-BMC-Blog-Structure.ps1`
**الوظيفة:** إنشاء بنية المحتوى الكاملة للمدونة

**المحتوى المنشأ:**
- ✅ Site Root: `/sitecore/content/BMC/BmcBlog`
- ✅ Home Page
- ✅ Blog Root مع المجلدات الفرعية
- ✅ 4 تصنيفات نموذجية (Technology, Business, Development, Cloud)
- ✅ 3 كتّاب نموذجيين
- ✅ 3 مقالات نموذجية
- ✅ Media Library: `/sitecore/media library/Project/BMC/BmcBlog`

**الميزات:**
- ✅ التحقق من العناصر الموجودة وتحديثها
- ✅ ربط Site بـ Media Library تلقائياً
- ✅ محتوى نموذجي جاهز للاختبار

---

## 🚀 ترتيب التشغيل الصحيح

### الخطوة 1️⃣: إنشاء Templates
```powershell
# في Sitecore PowerShell ISE
.\Create-BMC-Blog-Templates.ps1
```

**النتيجة المتوقعة:**
```
✓ Site Template
✓ Blog Root Template
✓ Blog Post Template
✓ Category Template
✓ Author Template
✓ Blog Listing Template
```

**التحقق:**
- افتح Content Editor
- انتقل إلى `/sitecore/templates/Project/BMC`
- تحقق من وجود `Site` template
- انتقل إلى `/sitecore/templates/Feature/BMC/Blog`
- تحقق من جميع templates

---

### الخطوة 2️⃣: استخراج Template IDs
```powershell
# في Sitecore PowerShell ISE
.\Update-Template-IDs.ps1
```

**النتيجة المتوقعة:**
```
✓ Site : {F2FD4169-6FF9-4A5B-826C-63A2F091E91E}
✓ BlogPost : {GUID-HERE}
✓ BlogRoot : {GUID-HERE}
✓ Category : {GUID-HERE}
✓ Author : {GUID-HERE}
```

**ما بعد التشغيل:**
1. انسخ محتوى ملف `Templates.cs` من المخرجات
2. الصق في: `Foundation/BMC.Foundation.SitecoreExtensions/Constants/Templates.cs`
3. احفظ الملف
4. أعد بناء المشروع

---

### الخطوة 3️⃣: إنشاء بنية المحتوى
```powershell
# في Sitecore PowerShell ISE
.\Create-BMC-Blog-Structure.ps1
```

**النتيجة المتوقعة:**
```
✓ موقع BmcBlog: /sitecore/content/BMC/BmcBlog
✓ صفحة Home: /sitecore/content/BMC/BmcBlog/Home
✓ Blog Root: /sitecore/content/BMC/BmcBlog/Home/Blog
✓ التصنيفات: 4
✓ الكتّاب: 3
✓ المقالات: 3
✓ مكتبة الوسائط: /sitecore/media library/Project/BMC/BmcBlog
```

**التحقق:**
- افتح Content Editor
- انتقل إلى `/sitecore/content/BMC/BmcBlog`
- تحقق من جميع العناصر المنشأة

---

## 📝 ملاحظات مهمة

### ⚠️ قبل التشغيل

1. **تأكد من وجود Sitecore PowerShell Extensions**
   - قم بتثبيته إذا لم يكن موجوداً
   - التحقق: افتح Sitecore Desktop → PowerShell ISE

2. **الصلاحيات المطلوبة**
   - يجب أن يكون لديك صلاحيات Administrator
   - صلاحيات الكتابة على `master` database

3. **النسخ الاحتياطي**
   - قم بعمل backup لـ `master` database قبل التشغيل
   - أو استخدم بيئة تطوير منفصلة

### 🔄 إعادة التشغيل

السكريبتات ذكية وتتحقق من العناصر الموجودة:
- ✅ إذا وُجد العنصر → يتم تحديثه
- ✅ إذا لم يوجد → يتم إنشاؤه

يمكنك تشغيل السكريبتات عدة مرات بأمان!

### 📊 بعد التشغيل

#### نشر إلى Web Database
```powershell
# نشر Templates
Publish-Item -Path "/sitecore/templates/Project/BMC" -Recurse -PublishMode Full -Target "web"
Publish-Item -Path "/sitecore/templates/Feature/BMC/Blog" -Recurse -PublishMode Full -Target "web"

# نشر المحتوى
Publish-Item -Path "/sitecore/content/BMC/BmcBlog" -Recurse -PublishMode Full -Target "web"

# نشر Media Library
Publish-Item -Path "/sitecore/media library/Project/BMC/BmcBlog" -Recurse -PublishMode Full -Target "web"
```

#### إعادة بناء الفهارس
```powershell
# إعادة بناء جميع الفهارس
Get-SearchIndex | ForEach-Object {
    Write-Host "Rebuilding: $($_.Name)"
    Initialize-SearchIndex -Name $_.Name
}
```

---

## 🐛 استكشاف الأخطاء

### المشكلة: "القالب غير موجود"
**الحل:**
1. تأكد من تشغيل `Create-BMC-Blog-Templates.ps1` أولاً
2. تحقق من المسارات في Content Editor
3. تأكد من نشر Templates

### المشكلة: "العنصر الأب غير موجود"
**الحل:**
1. تأكد من وجود `/sitecore/content/BMC`
2. قم بإنشاء المجلدات الأساسية يدوياً
3. أعد تشغيل السكريبت

### المشكلة: "خطأ في تحديث الحقول"
**الحل:**
1. تحقق من أسماء الحقول في Templates
2. تأكد من أن Field Types صحيحة
3. راجع Sitecore logs في: `/sitecore/admin/showlog.aspx`

### المشكلة: "Template ID غير صحيح"
**الحل:**
1. قم بتشغيل `Update-Template-IDs.ps1` مرة أخرى
2. تأكد من نسخ IDs الصحيحة
3. أعد بناء المشروع

---

## 🎯 التخصيص

### إضافة تصنيفات جديدة
عدّل المصفوفة `$sampleCategories` في `Create-BMC-Blog-Structure.ps1`:

```powershell
$sampleCategories = @(
    @{
        Name = "YourCategory"
        DisplayName = "التصنيف الجديد - Your Category"
        Description = "وصف التصنيف"
    },
    # أضف المزيد...
)
```

### إضافة كتّاب جدد
عدّل المصفوفة `$sampleAuthors`:

```powershell
$sampleAuthors = @(
    @{
        Name = "Author-Name"
        DisplayName = "اسم الكاتب"
        Biography = "السيرة الذاتية"
    },
    # أضف المزيد...
)
```

### إضافة مقالات جديدة
عدّل المصفوفة `$samplePosts`:

```powershell
$samplePosts = @(
    @{
        Name = "Post-Name"
        Title = "عنوان المقالة"
        Summary = "ملخص المقالة"
        Content = "<p>محتوى المقالة</p>"
        Category = "Technology"
    },
    # أضف المزيد...
)
```

---

## 📚 مراجع إضافية

### Sitecore PowerShell Extensions
- [الموقع الرسمي](https://doc.sitecorepowershell.com/)
- [GitHub Repository](https://github.com/SitecorePowerShell/Console)

### Sitecore Templates
- [Template Documentation](https://doc.sitecore.com/developers/102/platform-administration-and-architecture/en/templates.html)
- [Template Fields](https://doc.sitecore.com/developers/102/platform-administration-and-architecture/en/template-fields.html)

### BMC Blog Project
- راجع ملف `/IMPLEMENTATION_SUMMARY.md` للتفاصيل الكاملة
- راجع ملف `/ALIGNMENT_ANALYSIS.md` لتحليل البنية

---

## ✅ Checklist بعد التشغيل

- [ ] تم تشغيل `Create-BMC-Blog-Templates.ps1` بنجاح
- [ ] تم التحقق من Templates في Content Editor
- [ ] تم تشغيل `Update-Template-IDs.ps1`
- [ ] تم نسخ Template IDs إلى المشروع
- [ ] تم إعادة بناء المشروع
- [ ] تم تشغيل `Create-BMC-Blog-Structure.ps1`
- [ ] تم التحقق من المحتوى في Content Editor
- [ ] تم نشر Templates إلى web
- [ ] تم نشر المحتوى إلى web
- [ ] تم نشر Media Library إلى web
- [ ] تم إعادة بناء الفهارس
- [ ] تم اختبار الموقع على `https://abdo.sc`

---

## 📞 الدعم

إذا واجهت أي مشاكل:
1. راجع قسم "استكشاف الأخطاء" أعلاه
2. تحقق من Sitecore logs
3. راجع التوثيق في `/IMPLEMENTATION_SUMMARY.md`

---

**آخر تحديث:** 2025-11-23
**الإصدار:** 1.0
**المؤلف:** BMC Blog Team
