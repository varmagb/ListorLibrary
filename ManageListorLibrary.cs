using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using System.Windows.Forms;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.SharePoint.Client.Utilities;
using System.EnterpriseServices.Internal;

namespace ListOrLibrary_ASIS
{

    public static class FolderExtensions
    {


        /// <summary>
        /// Copy files 
        /// </summary>
        /// <param name="folder">Source Folder</param>
        /// <param name="folderUrl">Target Folder Url</param>
        public static void CopyFilesTo(this Folder folder, string folderUrl)
        {
            var ctx = (ClientContext)folder.Context;
            if (!ctx.Web.IsPropertyAvailable("ServerRelativeUrl"))
            {
                ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
            }
            ctx.Load(folder, f => f.Files, f => f.ServerRelativeUrl, f => f.Folders);
            ctx.ExecuteQuery();

            //Ensure target folder exists
            ctx.Web.EnsureFolder(folderUrl.Replace(ctx.Web.ServerRelativeUrl, string.Empty));
            foreach (var file in folder.Files)
            {
                var targetFileUrl = file.ServerRelativeUrl.Replace(folder.ServerRelativeUrl, folderUrl);
                file.CopyTo(targetFileUrl, true);
            }
            ctx.ExecuteQuery();

            foreach (var subFolder in folder.Folders)
            {
                var targetFolderUrl = subFolder.ServerRelativeUrl.Replace(folder.ServerRelativeUrl, folderUrl);
                subFolder.CopyFilesTo(targetFolderUrl);
            }
        }
    }

    static class WebExtensions
    {
        /// <summary>
        /// Ensures whether the folder exists   
        /// </summary>
        /// <param name="web"></param>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public static Folder EnsureFolder(this Web web, string folderUrl)
        {
            return EnsureFolderInternal(web.RootFolder, folderUrl);
        }


        private static Folder EnsureFolderInternal(Folder parentFolder, string folderUrl)
        {
            var ctx = parentFolder.Context;
            var folderNames = folderUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var folderName = folderNames[0];
            var folder = parentFolder.Folders.Add(folderName);
            ctx.Load(folder);
            ctx.ExecuteQuery();

            if (folderNames.Length > 1)
            {
                var subFolderUrl = string.Join("/", folderNames, 1, folderNames.Length - 1);
                return EnsureFolderInternal(folder, subFolderUrl);
            }
            return folder;
        }
    }


    public partial class ManageListorLibrary : System.Windows.Forms.Form
    {
       

        public ManageListorLibrary()
        {
            InitializeComponent();
        }

        private void btnAllListOrLibraryCopy_Click(object sender, EventArgs e)
        {
          //  Logging.Enter(typeof(ManageListorLibrary), "Create and copy all lists");
            try
            {
                ClientContext sourceContext = new ClientContext(this.txtSiteName.Text);
                sourceContext.Load<ListCollection>(sourceContext.Web.Lists, new Expression<Func<ListCollection, object>>[0]);
                sourceContext.ExecuteQuery();
                foreach (List list in sourceContext.Web.Lists)
                {
                    if ((((list.BaseTemplate == 100) && (list.BaseType == BaseType.GenericList)) && !list.Hidden) && !list.IsPrivate)
                    {
                        string title = list.Title;
                        string lstInternalName = ResolveListUrl(this.txtSiteName.Text, title);
                        this.MigrateListItemsWithAttachments(title, sourceContext, this.txtDSSiteName.Text, lstInternalName);
                        MessageBox.Show("All list created and data copied successfully");
                    }
                }
            }
            catch (Exception exception)
            {
         //       Logging.Exception("Create and copy all lists", exception);
                MessageBox.Show(exception.Message);
            }
          //  Logging.Leave(typeof(ManageList), "Create and copy all lists");
            Application.Exit();

        }



        //  private void CopyDocuments(string srcUrl, string srcLibrary, string destUrl, string destLibrary)
      //    {
      public static void CopyFiles(string url, string listTitle, string srcFolder,string destFolder)
  {
            using (var context = new ClientContext(url))
            {
                //context.Credentials = credentials;


                var srcList = context.Web.Lists.GetByTitle(listTitle);
                var qry = CamlQuery.CreateAllItemsQuery();
                qry.FolderServerRelativeUrl = string.Format("/{0}", srcFolder);
                var srcItems = srcList.GetItems(qry);
                context.Load(srcItems, icol => icol.Include(i => i.FileSystemObjectType, i => i["FileRef"], i => i.File));
                context.ExecuteQuery();

                foreach (var item in srcItems)
                {
                    switch (item.FileSystemObjectType)
                    {
                        case FileSystemObjectType.Folder:
                            var destFolderUrl = ((string)item["FileRef"]).Replace(srcFolder, destFolder);
                            CreateFolder(context.Web, destFolderUrl);
                            break;
                        case FileSystemObjectType.File:
                            var destFileUrl = item.File.ServerRelativeUrl.Replace(srcFolder, destFolder);
                            item.File.CopyTo(destFileUrl, true);
                            context.ExecuteQuery();
                            break;
                    }
                }
            }
        }

        private static Folder CreateFolder(Web web, string folderUrl)
        {
            if (string.IsNullOrEmpty(folderUrl))
                throw new ArgumentNullException("Folder Url could not be empty");

            var folder = web.Folders.Add(folderUrl);
            web.Context.Load(folder);
            web.Context.ExecuteQuery();
            return folder;
        }

        /*
        // set up the src client
        ClientContext srcContext = new ClientContext(srcUrl);

        // set up the destination context
        ClientContext destContext = new ClientContext(destUrl);

        // get the source list and items
        Web srcWeb = srcContext.Web;
        List srcList = srcWeb.Lists.GetByTitle(srcLibrary);
        ListItemCollection itemColl = srcList.GetItems(new CamlQuery());
        srcContext.Load(itemColl);
        srcContext.ExecuteQuery();

        // get the destination list
        Web destWeb = destContext.Web;
        destContext.Load(destWeb);
        destContext.ExecuteQuery();

        foreach (var doc in itemColl)
        {
            try
            {
                //if (doc.FileSystemObjectType == FileSystemObjectType.File) //Field or Property "FileAttachement not found."
                //{
                // get the file
                Microsoft.SharePoint.Client.File file = doc.File;
                srcContext.Load(file);
                srcContext.ExecuteQuery();

                // build destination url
                string nLocation = destWeb.ServerRelativeUrl.TrimEnd('/') + "/" + destLibrary.Replace(" ", "") + "/" + file.Name;

                // read the file, copy the content to new file at new location
                FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(srcContext, file.ServerRelativeUrl);
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(destContext, nLocation, fileInfo.Stream, true);
                // }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }*/


      

          


        private void btnCopyListOrLibraryData_Click(object sender, EventArgs e)
        {

            var srcFolderUrl = "http://share.d.niddk.nih.gov/sites/EOD/DOC1"; //News site with Pages library
            var destFolderUrl = "http://share.d.niddk.nih.gov/dea/CAMP/DESTDOC1"; //News archive sub site with Pages library
            using (var ctx = new ClientContext(txtSiteName.Text))
            {
                var sourceFolder = ctx.Web.GetFolderByServerRelativeUrl(srcFolderUrl);
                sourceFolder.CopyFilesTo(destFolderUrl);
            }

            //       Logging.Enter(typeof(ManageList), "Create List and Copy Data");
           // CopyFiles("https://contoso.sharepoint.com/", "Shared Documents", "Expense", "Expense 2015");
        //CopyDocuments(txtSiteName.Text,"DOC1" , txtDSSiteName.Text, "DOC1");
        /*    try
            {
                ClientContext sourceContext = new ClientContext(this.txtSiteName.Text);
                string lstInternalName = ResolveListUrl(this.txtSiteName.Text, this.txtListName.Text);
                this.MigrateListItemsWithAttachments(this.txtListName.Text, sourceContext, this.txtDSSiteName.Text, lstInternalName);
                MessageBox.Show("List created and data copied successfully");
            }
            catch (Exception exception)
            {
       //         Logging.Exception("Create List and Copy Data", exception);
                MessageBox.Show(exception.Message);
            } */
        //   Logging.Leave(typeof(ManageList), "Create List and Copy Data");
        Application.Exit();

        }

        private void btnIncrementalCopy_Click(object sender, EventArgs e)
        {
        //    Logging.Enter(typeof(ManageList), "Incremental Copy");
            List sourceList = null;
            List destinationList = null;
            ListItemCollection clientObject = null;
            ListItemCollection items = null;
            try
            {
                ClientContext context = new ClientContext(this.txtSiteName.Text);
                ClientContext destClientContext = new ClientContext(this.txtDSSiteName.Text);
                sourceList = context.Web.Lists.GetByTitle(this.txtListName.Text);
                destinationList = destClientContext.Web.Lists.GetByTitle(this.txtListName.Text);
                clientObject = sourceList.GetItems(CamlQuery.CreateAllItemsQuery());
                items = destinationList.GetItems(CamlQuery.CreateAllItemsQuery());
                context.Load<ListItemCollection>(clientObject, new Expression<Func<ListItemCollection, object>>[0]);
                context.Load<FieldCollection>(sourceList.Fields, new Expression<Func<FieldCollection, object>>[0]);
                context.ExecuteQuery();
                destClientContext.Load<ListItemCollection>(items, new Expression<Func<ListItemCollection, object>>[0]);
                destClientContext.Load<FieldCollection>(destinationList.Fields, new Expression<Func<FieldCollection, object>>[0]);
                destClientContext.ExecuteQuery();
                string str = string.Empty;
                if (clientObject.Count == items.Count)
                {
                    str = "ItemModified";
                }
                else if (clientObject.Count > items.Count)
                {
                    str = "ItemAdded";
                }
                else
                {
                    str = "ItemDeleted";
                }
                string str2 = str;
                if (str2 != "ItemModified")
                {
                    if (str2 == "ItemAdded")
                    {
                        goto Label_0178;
                    }
                    if (str2 == "ItemDeleted")
                    {
                        goto Label_0187;
                    }
                }
                else
                {
                    this.ItemModified(clientObject, items, sourceList, destinationList, destClientContext);
                }
                goto Label_0194;
            Label_0178:
                this.ItemAdded(clientObject, items, sourceList, destinationList, destClientContext);
                goto Label_0194;
            Label_0187:
                this.ItemDeleted(clientObject, items, destClientContext);
            Label_0194:
                MessageBox.Show("List incremental modification completed successfully");
            }
            catch (Exception exception)
            {
        //        Logging.Exception("Incremental Copy", exception);
                MessageBox.Show(exception.Message);
            }
    //        Logging.Leave(typeof(ManageList), "Incremental Copy");
            Application.Exit();

        }

        private void btnCreateEmptyList_Click(object sender, EventArgs e)
        {
     //       Logging.Enter(typeof(ManageList), "Copy Empty List and Fields");
            ClientContext sourceContext = new ClientContext(this.txtSiteName.Text);
            string lstInternalName = ResolveListUrl(this.txtSiteName.Text, this.txtListName.Text);
            List sourceList = null;
            ListItemCollection clientObject = null;
            List destinationList = null;
            try
            {
                sourceList = sourceContext.Web.Lists.GetByTitle(this.txtListName.Text);
                clientObject = sourceList.GetItems(CamlQuery.CreateAllItemsQuery());
                FieldCollection fields = sourceContext.Web.Lists.GetByTitle(this.txtListName.Text).Fields;
                sourceContext.Load<ListItemCollection>(clientObject, new Expression<Func<ListItemCollection, object>>[0]);
                sourceContext.Load<FieldCollection>(fields, new Expression<Func<FieldCollection, object>>[0]);
                sourceContext.ExecuteQuery();
                ClientContext destContext = new ClientContext(this.txtDSSiteName.Text);
                destinationList = this.CreateList(sourceList, sourceContext, fields, destContext, this.txtDSSiteName.Text, lstInternalName, destinationList, this.txtListName.Text);
                MessageBox.Show("Empty List created successfully");
            }
            catch (Exception exception)
            {
     //           Logging.Exception("Copy Empty List and Fields", exception);
                MessageBox.Show(exception.Message);
            }
     //       Logging.Leave(typeof(ManageList), "Copy Empty List and Fields");
            Application.Exit();

        }


        private List CreateList(List sourceList, ClientContext sourceContext, FieldCollection fldcoll, ClientContext destContext, string strDestSite, string lstInternalName, List destinationList, string strListName)
        {
            // List listByTitleCS = GetListByTitleCS(destContext, strListName);
            List listByTitleCS = sourceContext.Web.Lists.GetByTitle(strListName); ;
            Web web = destContext.Web;
        //    if (listByTitleCS == null)
        //    {
                ListCreationInformation parameters = new ListCreationInformation
                {
                    Title = lstInternalName,
                    TemplateType = 100
                };
                List clientObject = web.Lists.Add(parameters);
                clientObject.Description = lstInternalName + "Description";
                destinationList = destContext.Web.Lists.GetByTitle(lstInternalName);
                destContext.Load<ViewCollection>(destinationList.Views, new Expression<Func<ViewCollection, object>>[0]);
                destContext.ExecuteQuery();
                sourceContext.Load<ViewCollection>(sourceList.Views, new Expression<Func<ViewCollection, object>>[0]);
                sourceContext.ExecuteQuery();
                Microsoft.SharePoint.Client.View view = sourceList.Views[0];
                ViewFieldCollection viewFields = view.ViewFields;
                sourceContext.Load<ViewFieldCollection>(viewFields, new Expression<Func<ViewFieldCollection, object>>[0]);
                sourceContext.ExecuteQuery();
                foreach (Field field in fldcoll)
                {
                    if (!field.FromBaseType)
                    {
                        object[] objArray1 = new object[] { "<Field DisplayName= '", field.InternalName, "'  Name='", field.InternalName, "' Type='", field.FieldTypeKind, "' />" };
                        clientObject.Fields.AddFieldAsXml(string.Concat(objArray1), true, AddFieldOptions.DefaultValue);
                    }
                    else if ((field.InternalName == "Created") && view.ViewFields.Contains<string>("Created"))
                    {
                        Microsoft.SharePoint.Client.View view2 = destinationList.Views[0];
                        view2.ViewFields.Add("Created");
                        view2.Update();
                    }
                    else if ((field.InternalName == "Author") && view.ViewFields.Contains<string>("Author"))
                    {
                        Microsoft.SharePoint.Client.View view3 = destinationList.Views[0];
                        view3.ViewFields.Add("Created By");
                        view3.Update();
                    }
                    else if ((field.InternalName == "Modified") && view.ViewFields.Contains<string>("Modified"))
                    {
                        Microsoft.SharePoint.Client.View view4 = destinationList.Views[0];
                        view4.ViewFields.Add("Modified");
                        view4.Update();
                    }
                    else if ((field.InternalName == "Editor") && view.ViewFields.Contains<string>("Editor"))
                    {
                        Microsoft.SharePoint.Client.View view5 = destinationList.Views[0];
                        view5.ViewFields.Add("Modified By");
                        view5.Update();
                    }
                }
                destinationList = destContext.Web.Lists.GetByTitle(lstInternalName);
                destContext.Load<FieldCollection>(destinationList.Fields, new Expression<Func<FieldCollection, object>>[0]);
                destContext.ExecuteQuery();
                clientObject.Title = strListName;
                clientObject.Update();
                destContext.Load<List>(clientObject, new Expression<Func<List, object>>[0]);
                destContext.ExecuteQuery();
                return destinationList;
          //  }
            destinationList = destContext.Web.Lists.GetByTitle(strListName);
            destContext.Load<FieldCollection>(destinationList.Fields, new Expression<Func<FieldCollection, object>>[0]);
            destContext.ExecuteQuery();
          //  this.DeleteAllListItems(destContext, destinationList);
            return destinationList;
        }

      

      

     /*   public void DeleteAllListItems(ClientContext destContext, List destList)
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            ListItemCollection clientObject = destList.GetItems(CamlQuery.CreateAllItemsQuery());
            Expression<Func<ListItemCollection, object>>[] retrievals = new Expression<Func<ListItemCollection, object>>[1];
            Expression[] arguments = new Expression[2];
            arguments[0] = expression = Expression.Parameter(typeof(ListItemCollection), "eachItem");
            Expression[] initializers = new Expression[2];
            ParameterExpression[] parameters = new ParameterExpression[] { expression2 };
            initializers[0] = Expression.Quote(Expression.Lambda<Func<ListItem, object>>(expression2 = Expression.Parameter(typeof(ListItem), "item"), parameters));
            Expression[] expressionArray5 = new Expression[] { Expression.Constant("ID", typeof(string)) };
            ParameterExpression[] expressionArray6 = new ParameterExpression[] { expression2 };
            initializers[1] = Expression.Quote(Expression.Lambda<Func<ListItem, object>>(Expression.Call(expression2 = Expression.Parameter(typeof(ListItem), "item"), (MethodInfo)methodof(ListItem.get_Item), expressionArray5), expressionArray6));
            arguments[1] = Expression.NewArrayInit(typeof(Expression<Func<ListItem, object>>), initializers);
            ParameterExpression[] expressionArray7 = new ParameterExpression[] { expression };
            retrievals[0] = Expression.Lambda<Func<ListItemCollection, object>>(Expression.Call(null, (MethodInfo)methodof(ClientObjectQueryableExtension.Include), arguments), expressionArray7);
            destContext.Load<ListItemCollection>(clientObject, retrievals);
            destContext.ExecuteQuery();
            int count = clientObject.Count;
            Console.WriteLine("Deletion in " + destList + "list:");
            if (count > 0)
            {
                for (int i = count - 1; i > -1; i--)
                {
                    clientObject[i].DeleteObject();
                }
                destContext.ExecuteQuery();
            }
        }*/

        

        private void ItemAdded(ListItemCollection sourceItems, ListItemCollection destItems, List sourceList, List destinationList, ClientContext destClientContext)
        {
            for (int i = destItems.Count; i < sourceItems.Count; i++)
            {
                ListItemCreationInformation parameters = new ListItemCreationInformation();
                ListItem clientObject = destinationList.AddItem(parameters);
                for (int j = destItems.Count; j < sourceItems.Count; j++)
                {
                    foreach (Field field in sourceList.Fields)
                    {
                        if (((!field.ReadOnlyField && !field.Hidden) && ((field.InternalName != "Attachments") && (field.InternalName != "ContentType"))) || ((((field.InternalName == "Modified") || (field.InternalName == "Editor")) || (field.InternalName == "Created")) || (field.InternalName == "Author")))
                        {
                            try
                            {
                                if (field.FieldTypeKind == FieldType.Note)
                                {
                                    clientObject[field.InternalName] = Regex.Replace(sourceItems[i][field.InternalName].ToString(), "<[^>]*>", string.Empty);
                                }
                                else
                                {
                                    clientObject[field.InternalName] = sourceItems[i][field.InternalName];
                                }
                            }
                            catch (Exception exception)
                            {
                     //           Logging.Exception("MigrateListItemsWithAttachments", exception);
                            }
                        }
                    }
                    clientObject.Update();
                    destClientContext.Load<ListItem>(clientObject, new Expression<Func<ListItem, object>>[0]);
                    destClientContext.ExecuteQuery();
                }
            }
        }

        

        private void ItemDeleted(ListItemCollection sourceItems, ListItemCollection destItems, ClientContext destClientContext)
        {
            for (int i = sourceItems.Count; i < destItems.Count; i++)
            {
                destItems[i].DeleteObject();
                destClientContext.Load<ListItemCollection>(destItems, new Expression<Func<ListItemCollection, object>>[0]);
                destClientContext.ExecuteQuery();
            }
        }


        private void ItemModified(ListItemCollection sourceItems, ListItemCollection destItems, List sourceList, List destinationList, ClientContext destClientContext)
        {
            for (int i = 0; i < sourceItems.Count; i++)
            {
                DateTime time = (DateTime)sourceItems[i]["Modified"];
                for (int j = i; j < destItems.Count; j++)
                {
                    DateTime time2 = (DateTime)destItems[j]["Modified"];
                    if (!(time != time2))
                    {
                        break;
                    }
                    foreach (Field field in destinationList.Fields)
                    {
                        if ((((!field.ReadOnlyField && !field.Hidden) && ((field.InternalName != "Attachments") && (field.InternalName != "ContentType"))) && ((field.InternalName != "Editor") && (field.InternalName != "Created"))) && (field.InternalName != "Author"))
                        {
                            try
                            {
                                destItems[j][field.InternalName] = sourceItems[i][field.InternalName];
                                destItems[j].Update();
                                destClientContext.Load<FieldCollection>(destinationList.Fields, new Expression<Func<FieldCollection, object>>[0]);
                                destClientContext.Load<ListItemCollection>(destItems, new Expression<Func<ListItemCollection, object>>[0]);
                                destClientContext.ExecuteQuery();
                            }
                            catch (Exception exception)
                            {
                          //      Logging.Exception("MigrateListItemsWithAttachments", exception);
                            }
                        }
                    }
                }
            }
        }


        

        private void MigrateListItemsWithAttachments(string strListName, ClientContext sourceContext, string destSite, string lstInternalName)
        {
         //   Logging.Enter(typeof(ManageList), "MigrateListItemsWithAttachments");
            List sourceList = null;
            ListItemCollection clientObject = null;
            List destinationList = null;
            try
            {
                sourceList = sourceContext.Web.Lists.GetByTitle(strListName);
                clientObject = sourceList.GetItems(CamlQuery.CreateAllItemsQuery());
                FieldCollection fields = sourceContext.Web.Lists.GetByTitle(strListName).Fields;
                sourceContext.Load<ListItemCollection>(clientObject, new Expression<Func<ListItemCollection, object>>[0]);
                sourceContext.Load<FieldCollection>(fields, new Expression<Func<FieldCollection, object>>[0]);
                sourceContext.ExecuteQuery();
                ClientContext destContext = new ClientContext(destSite);
                destinationList = this.CreateList(sourceList, sourceContext, fields, destContext, destSite, lstInternalName, destinationList, strListName);
                foreach (ListItem item in clientObject)
                {
                    ListItemCreationInformation parameters = new ListItemCreationInformation();
                    ListItem item2 = destinationList.AddItem(parameters);
                    AttachmentCollection attachmentFiles = item.AttachmentFiles;
                    AttachmentCollection attachmentFiles2 = attachmentFiles;
                    foreach (Field field in destinationList.Fields)
                    {
                        if (((!field.ReadOnlyField && !field.Hidden) && ((field.InternalName != "Attachments") && (field.InternalName != "ContentType"))) || ((((field.InternalName == "Modified") || (field.InternalName == "Editor")) || (field.InternalName == "Created")) || (field.InternalName == "Author")))
                        {
                            try
                            {
                                if (field.FieldTypeKind == FieldType.Note)
                                {
                                    item2[field.InternalName] = Regex.Replace(item[field.InternalName].ToString(), "<[^>]*>", string.Empty);
                                }
                                else
                                {
                                    item2[field.InternalName] = item[field.InternalName];
                                }
                            }

                           
                            catch (Exception exception)
                            {
                                //          Logging.Exception("MigrateListItemsWithAttachments", exception);
                            }
                        }
                    }
                    item2.Update();
                    destContext.Load<ListItem>(item2, new Expression<Func<ListItem, object>>[0]);
                    destContext.ExecuteQuery();
                    UpdateAttachments
                          (sourceContext, destContext, item.Id, item2.Id, strListName);



                }
            }
            catch (Exception exception2)
            {
       //         Logging.Exception("MigrateListItemsWithAttachments", exception2);
            }
       //     Logging.Leave(typeof(ManageList), "MigrateListItemsWithAttachments");
        }



        private static void UpdateAttachments(ClientContext srccontext,
              ClientContext dstcontext, int srcItemID, int destItemID, string listName)
        {
            try
            {
               
                
                
                //getting attachment from files
                Web srcweb = srccontext.Web;
                srccontext.Load(srcweb);
                srccontext.ExecuteQuery();
                string src = string.Format("{0}/lists/{1}/Attachments/{2}",
                                  srcweb.Url, listName, srcItemID);
                Folder attachmentsFolder = srcweb.GetFolderByServerRelativeUrl(src);
                srccontext.Load(attachmentsFolder);
                FileCollection attachments = attachmentsFolder.Files;
                srccontext.Load(attachments);
                srccontext.ExecuteQuery();

                if (attachments.Count > 0)
                {
                    foreach (Microsoft.SharePoint.Client.File attachment in attachments)
                    {
                        ClientResult<Stream> clientResultStream = attachment.OpenBinaryStream();
                        srccontext.ExecuteQuery();
                        var stream = clientResultStream.Value;

                        AttachmentCreationInformation attachFileInfo =
                                                     new AttachmentCreationInformation();
                        Byte[] buffer = new Byte[attachment.Length];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        System.IO.MemoryStream stream2 = new System.IO.MemoryStream(buffer);
                        attachFileInfo.ContentStream = stream2;
                        attachFileInfo.FileName = attachment.Name;

                        Web destweb = dstcontext.Web;
                        List destlist = destweb.Lists.GetByTitle(listName);
                        ListItem destitem = destlist.GetItemById(destItemID);
                        dstcontext.Load(destitem);
                        dstcontext.ExecuteQuery();
                        Attachment a = destitem.AttachmentFiles.Add(attachFileInfo);
                        dstcontext.Load(a);
                        dstcontext.ExecuteQuery();
                        stream2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //Log exception
            }
        }


        public static string ResolveListUrl(string url, string listTitle)
        {
            using (ClientContext context = new ClientContext(url))
            {
                List byTitle = context.Web.Lists.GetByTitle(listTitle);
                context.Load<Folder>(byTitle.RootFolder, new Expression<Func<Folder, object>>[0]);
                context.ExecuteQuery();
                return byTitle.RootFolder.Name;
            }
        }
        


}
}
