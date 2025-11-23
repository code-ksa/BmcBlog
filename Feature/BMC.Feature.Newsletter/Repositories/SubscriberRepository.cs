using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace BMC.Feature.Newsletter.Repositories
{
    public class SubscriberRepository
    {
        private readonly Database _database;
        private const string SubscribersPath = "/sitecore/content/BMC/SA/Blog/Data/Subscribers";
        private static readonly ID SubscriberTemplateId = new ID("{12345678-1234-1234-1234-123456789012}");

        public SubscriberRepository()
        {
            _database = Sitecore.Context.Database ?? Database.GetDatabase("master");
        }

        public Item AddSubscriber(string email, string name)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            try
            {
                var subscribersFolder = _database.GetItem(SubscribersPath);
                if (subscribersFolder == null)
                {
                    subscribersFolder = CreateSubscribersFolder();
                    if (subscribersFolder == null)
                        return null;
                }

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    var itemName = ItemUtil.ProposeValidItemName(email.Replace("@", "_").Replace(".", "_"));
                    var template = _database.GetTemplate(SubscriberTemplateId) ?? _database.GetTemplate(Sitecore.TemplateIDs.Folder);

                    var subscriber = subscribersFolder.Add(itemName, template);

                    if (subscriber != null)
                    {
                        subscriber.Editing.BeginEdit();
                        
                        if (subscriber.Fields["Email"] != null)
                            subscriber.Fields["Email"].Value = email;
                        
                        if (!string.IsNullOrEmpty(name) && subscriber.Fields["Name"] != null)
                            subscriber.Fields["Name"].Value = name;
                        
                        if (subscriber.Fields["Subscription Date"] != null)
                            subscriber.Fields["Subscription Date"].Value = Sitecore.DateUtil.ToIsoDate(DateTime.Now);
                        
                        if (subscriber.Fields["Status"] != null)
                            subscriber.Fields["Status"].Value = "Active";

                        subscriber.Editing.EndEdit();
                    }

                    return subscriber;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool RemoveSubscriber(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var subscriber = GetSubscriberByEmail(email);
                if (subscriber == null)
                    return false;

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    subscriber.Delete();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Item GetSubscriberByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            try
            {
                var subscribersFolder = _database.GetItem(SubscribersPath);
                if (subscribersFolder == null)
                    return null;

                var subscribers = subscribersFolder.Children;
                foreach (Item subscriber in subscribers)
                {
                    var emailField = subscriber.Fields["Email"];
                    if (emailField != null && emailField.Value.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        return subscriber;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<Item> GetAllSubscribers()
        {
            try
            {
                var subscribersFolder = _database.GetItem(SubscribersPath);
                if (subscribersFolder == null)
                    return new List<Item>();

                return subscribersFolder.Children.ToList();
            }
            catch
            {
                return new List<Item>();
            }
        }

        private Item CreateSubscribersFolder()
        {
            try
            {
                var blogRoot = _database.GetItem("/sitecore/content/BMC/SA/Blog");
                if (blogRoot == null)
                    return null;

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    var dataFolder = blogRoot.Children["Data"];
                    if (dataFolder == null)
                    {
                        dataFolder = blogRoot.Add("Data", new TemplateID(Sitecore.TemplateIDs.Folder));
                    }

                    if (dataFolder != null)
                    {
                        var subscribersFolder = dataFolder.Add("Subscribers", new TemplateID(Sitecore.TemplateIDs.Folder));
                        return subscribersFolder;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}