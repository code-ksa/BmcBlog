# تقرير توفيق مشروع BMC Blog

## التاريخ: 2025-11-23

## 1. تحليل الفجوات (Gap Analysis)

### أ. إعدادات Sitecore غير متطابقة

**المشكلة الحالية:**
```xml
rootPath="/sitecore/content/BMC/SA/Blog"
```

**المطلوب (من لقطة الشاشة):**
```
Item path: /sitecore/content/BMC/BmcBlog
```

### ب. ملفات مفقودة مرجعية في الإعدادات

الملفات التالية مذكورة في `Feature.Blog.config` لكنها غير موجودة:

1. **BMC.Feature.Blog.Pipelines.ResolveBlogPost**
   - مرجع في: Line 22-23 من Feature.Blog.config
   - الوظيفة: حل عناصر المدونة في pipeline الـ httpRequestBegin

2. **BMC.Feature.Blog.Pipelines.CacheBlogRendering**
   - مرجع في: Line 27-28 من Feature.Blog.config
   - الوظيفة: إدارة الـ cache للعروض (renderings)

3. **BMC.Feature.Blog.Events.InvalidateBlogCache**
   - مرجع في: Line 34, 37 من Feature.Blog.config
   - الوظيفة: إبطال الـ cache عند حفظ أو حذف العناصر

4. **BMC.Feature.Blog.DependencyInjection.ServicesConfigurator**
   - مرجع في: Line 42 من Feature.Blog.config
   - الوظيفة: تكوين خدمات الـ Dependency Injection

### ج. قالب الموقع (Site Template)

**من لقطة الشاشة:**
```
Template: /sitecore/templates/Project/BMC/Site - {F2FD4169-6FF9-4A5B-826C-63A2F091E91E}
```

**مكتبة الوسائط:**
```
Site Media Library: sitecore/media library/Project/BMC/BmcBlog
```

## 2. خطة التوفيق

### المرحلة 1: تحديث الإعدادات
- تحديث مسار الموقع في `Project.BlogSite.config`
- تحديث اسم المضيف (hostname) إذا لزم الأمر
- مواءمة إعدادات مكتبة الوسائط

### المرحلة 2: إضافة الملفات المفقودة
- إنشاء ResolveBlogPost pipeline processor
- إنشاء CacheBlogRendering pipeline processor
- إنشاء InvalidateBlogCache event handler
- إنشاء ServicesConfigurator للـ DI

### المرحلة 3: تحديث Constants
- إضافة معرفات القوالب (Template IDs)
- إضافة معرفات الحقول (Field IDs)

### المرحلة 4: التحقق والاختبار
- التأكد من تطابق جميع المسارات
- التحقق من صحة المراجع

## 3. التوصيات

1. **مراجعة الهيكل الحالي:** التأكد من أن مسار `/sitecore/content/BMC/BmcBlog` هو الصحيح
2. **توحيد التسميات:** استخدام "BmcBlog" بدلاً من "SA/Blog"
3. **إكمال التطبيق:** إضافة جميع الملفات المرجعية المفقودة
4. **إعدادات اللغة:** التأكد من دعم اللغتين العربية والإنجليزية
5. **مكتبة الوسائط:** التأكد من تكوين المسار الصحيح

## 4. الملفات التي تحتاج إلى تحديث

### ملفات الإعدادات:
- ✓ `Project/BMC.Project.BlogSite/App_Config/Include/Project/Project.BlogSite.config`

### ملفات جديدة مطلوبة:
- ✗ `Feature/BMC.Feature.Blog/Pipelines/ResolveBlogPost.cs`
- ✗ `Feature/BMC.Feature.Blog/Pipelines/CacheBlogRendering.cs`
- ✗ `Feature/BMC.Feature.Blog/Events/InvalidateBlogCache.cs`
- ✗ `Feature/BMC.Feature.Blog/DependencyInjection/ServicesConfigurator.cs`

### ملفات محتملة للتحديث:
- ⚠ `Foundation/BMC.Foundation.SitecoreExtensions/Constants/Templates.cs`

## 5. البنية المستهدفة

```
/sitecore/content/BMC/
└── BmcBlog (Site Root)
    ├── Home
    ├── Blog (Blog Root)
    │   ├── Posts
    │   ├── Categories
    │   └── Authors
    └── Settings

/sitecore/media library/Project/BMC/
└── BmcBlog

/sitecore/templates/Project/BMC/
└── Site
```

## 6. الخطوات التالية

1. الحصول على موافقة على التغييرات المقترحة
2. تنفيذ التحديثات على الإعدادات
3. إنشاء الملفات المفقودة
4. اختبار التكامل
5. نشر التغييرات
