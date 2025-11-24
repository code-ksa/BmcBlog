# BMC Blog - VS Code Prompts for Claude
# Use these prompts sequentially in Visual Studio Code with GitHub Copilot

---

## STEP 1: إنشاء Blog Layout Item Configuration

**Prompt:**
```
أريدك أن تنشئ ملف C# helper class اسمه LayoutItemManager.cs في المسار:
src/Foundation/SitecoreExtensions/code/Helpers/

الملف يجب أن يحتوي على:
1. Class لإنشاء Layout items في Sitecore
2. Method لتسجيل Layout في Sitecore بالمعلومات التالية:
   - Path: /sitecore/layout/Layouts/BMC/Blog Layout
   - Physical Path: ~/Views/Layouts/BlogLayout.cshtml
   - Template: Layout (/sitecore/templates/System/Layout/Layout)

3. Method لربط Layout بـ Template معين
4. Method للتحقق من وجود Layout

استخدم Sitecore APIs النظيفة بدون Glass Mapper.
```

---

## STEP 2: إنشاء Rendering Items Manager

**Prompt:**
```
أريدك أن تنشئ ملف C# helper class اسمه RenderingItemManager.cs في المسار:
src/Foundation/SitecoreExtensions/code/Helpers/

الملف يجب أن يحتوي على:
1. Method لإنشاء View Rendering في Sitecore
2. Method لإنشاء Controller Rendering
3. Method لربط Rendering بـ Placeholder معين

المعلومات المطلوبة للـ Renderings:
- Header Rendering (Controller: BmcNavigation, Action: Header)
- Footer Rendering (Controller: BmcNavigation, Action: Footer)
- Breadcrumb Rendering (Controller: BmcNavigation, Action: Breadcrumb)
- Newsletter Rendering (Controller: BmcNewsletter, Action: Subscribe)

كل الـ Renderings يجب أن تكون في المسار:
/sitecore/layout/Renderings/BMC/
```

---

## STEP 3: إنشاء Presentation Manager

**Prompt:**
```
أريدك أن تنشئ ملف C# helper class اسمه PresentationManager.cs في المسار:
src/Foundation/SitecoreExtensions/code/Helpers/

الملف يجب أن يحتوي على:
1. Method لربط Layout بـ Sitecore item معين
2. Method لإضافة Rendering لـ Placeholder في item معين
3. Method لإضافة عدة Renderings دفعة واحدة
4. Method لحذف كل Presentation Details من item
5. Method للحصول على Presentation Details الحالية لـ item

استخدم DeviceItem و LayoutDefinition و RenderingReference من Sitecore API.
```

---

## STEP 4: إنشاء Template Manager

**Prompt:**
```
أريدك أن تنشئ ملف C# helper class اسمه TemplateManager.cs في المسار:
src/Foundation/SitecoreExtensions/code/Helpers/

الملف يجب أن يحتوي على:
1. Method لإنشاء Template folder
2. Method لإنشاء Template item
3. Method لإضافة Section لـ Template
4. Method لإضافة Field لـ Section
5. Method لتعيين Standard Values لـ Template
6. Method لتعيين Layout في Standard Values

المطلوب إنشاء Templates التالية في المسار /sitecore/templates/BMC/:
- Blog Post Template (يحتوي على: Title, Content, Author, PublishDate, Category, Tags, FeaturedImage)
- Blog Home Template (يحتوي على: Title, IntroText)
```

---

## STEP 5: إنشاء Sitecore Initializer

**Prompt:**
```
أريدك أن تنشئ ملف C# class اسمه SitecoreInitializer.cs في المسار:
src/Foundation/SitecoreExtensions/code/Infrastructure/

الملف يجب أن يحتوي على:
1. Static method Initialize() تستخدم كل الـ Managers السابقة
2. خطوات التنفيذ بالترتيب:
   - إنشاء Layout item
   - إنشاء Rendering items
   - إنشاء Templates
   - ربط Layout بـ Templates
   - ربط Layout بالصفحات الموجودة (Home, Blog)

3. يجب أن يكون الكود safe ويتحقق من وجود العناصر قبل إنشائها
4. استخدم Logging لتسجيل كل خطوة
5. استخدم Try-Catch للتعامل مع الأخطاء

يجب أن يكون هذا الكود قابل للتشغيل من Pipeline processor أو من PowerShell script.
```

---

## STEP 6: إنشاء Pipeline Processor

**Prompt:**
```
أريدك أن تنشئ ملف C# class اسمه InitializeBlogProcessor.cs في المسار:
src/Foundation/SitecoreExtensions/code/Pipelines/

الملف يجب أن يحتوي على:
1. Processor يرث من Sitecore.Pipelines.HttpRequest.HttpRequestProcessor
2. يستدعي SitecoreInitializer.Initialize()
3. يتحقق من علامة معينة (مثل query string ?initBlog=true) قبل التنفيذ
4. يعمل مرة واحدة فقط ثم يحفظ علامة في Database

أيضاً أنشئ ملف config اسمه Foundation.SitecoreExtensions.Blog.config في المسار:
src/Foundation/SitecoreExtensions/code/App_Config/Include/Foundation/

الملف يحتوي على تسجيل الـ Processor في initialize pipeline.
```

---

## STEP 7: إضافة Extensions Methods

**Prompt:**
```
أريدك أن تنشئ ملف C# extension class اسمه ItemExtensions.cs في المسار:
src/Foundation/SitecoreExtensions/code/Extensions/

الملف يجب أن يحتوي على Extension Methods لـ Sitecore Item:
1. HasLayout() - للتحقق من وجود Layout
2. GetLayout() - للحصول على Layout
3. SetLayout() - لتعيين Layout
4. AddRendering() - لإضافة Rendering
5. RemoveRendering() - لحذف Rendering
6. GetRenderings() - للحصول على كل Renderings
7. HasPresentation() - للتحقق من وجود أي Presentation

كل الـ Methods يجب أن تكون null-safe وتتعامل مع الأخطاء بشكل صحيح.
```

---

## STEP 8: Update Controllers

**Prompt:**
```
أريدك أن تراجع ملفات الـ Controllers التالية وتتأكد من صحتها:

1. src/Feature/Navigation/code/Controllers/BmcNavigationController.cs
   - تأكد من وجود Actions: Header, Footer, Breadcrumb
   - تأكد من الـ Views paths

2. src/Feature/Newsletter/code/Controllers/BmcNewsletterController.cs
   - تأكد من وجود Action: Subscribe
   - تأكد من الـ View path

3. تأكد من أن كل الـ Views موجودة في:
   - Views/BmcNavigation/
   - Views/BmcNewsletter/

إذا كانت هناك أي مشاكل، أصلحها.
```

---

## STEP 9: Create Build Script

**Prompt:**
```
أريدك أن تنشئ PowerShell script اسمه Build-And-Deploy.ps1 في المسار:
scripts/

الملف يجب أن يقوم بـ:
1. Build Solution كاملة
2. نسخ DLLs إلى bin folder في abdo.sc
3. نسخ Views إلى المسار الصحيح
4. نسخ Config files
5. عمل Clean للـ temp folders

يجب أن يحتوي على error handling وتسجيل كل خطوة.
```

---

## STEP 10: Create Testing Script

**Prompt:**
```
أريدك أن تنشئ PowerShell script اسمه Test-SitecoreSetup.ps1 في المسار:
scripts/

الملف يجب أن يتحقق من:
1. وجود Layout item في Sitecore
2. وجود Rendering items
3. وجود Templates
4. ربط Layout بالصفحات
5. وجود الملفات الفعلية (Views, DLLs)

يجب أن يعطي تقرير واضح عن حالة كل شيء.
```

---

## ملاحظات مهمة للاستخدام:

### كيفية استخدام هذه الـ Prompts:

1. **افتح VS Code** مع GitHub Copilot مفعل
2. **استخدم الـ Prompts بالترتيب** (STEP 1 → STEP 10)
3. **لكل Prompt:**
   - انسخ الـ Prompt كاملاً
   - افتح Copilot Chat في VS Code
   - الصق الـ Prompt
   - راجع الكود المُنشأ
   - اطلب تعديلات إذا لزم الأمر

### بعد إنهاء كل الـ Steps:

1. اعمل **Build** للـ Solution
2. شغل **Build-And-Deploy.ps1**
3. شغل **Test-SitecoreSetup.ps1** للتحقق
4. شغل سكريبتات PowerShell لـ Sitecore (الملف التالي)

---

## Tips لـ Claude في VS Code:

- إذا طلب منك Claude توضيح، أعطه معلومات عن:
  * Sitecore Version: 9.3
  * MVC Version: 5.2.4
  * .NET Framework: 4.7.2
  * هيكل المشروع: Helix Architecture

- إذا واجه Claude مشكلة في فهم الـ Sitecore APIs، اطلب منه:
  "استخدم Sitecore.Data.Items.Item و Sitecore.Data.Database بدون Glass Mapper"

- احفظ كل الملفات المُنشأة في Git قبل الـ Build

---

تاريخ الإنشاء: 2024-11-24
المشروع: BMC Blog
الهيكل: Sitecore 9.3 + Helix
