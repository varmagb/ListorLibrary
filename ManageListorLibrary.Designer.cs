namespace ListOrLibrary_ASIS
{
    partial class ManageListorLibrary
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtSiteName = new System.Windows.Forms.TextBox();
            this.lblListorLibrary = new System.Windows.Forms.Label();
            this.txtListName = new System.Windows.Forms.TextBox();
            this.lblDestination = new System.Windows.Forms.Label();
            this.txtDSSiteName = new System.Windows.Forms.TextBox();
            this.btnCreateEmptyList = new System.Windows.Forms.Button();
            this.btnCopyListOrLibraryData = new System.Windows.Forms.Button();
            this.btnIncrementalCopy = new System.Windows.Forms.Button();
            this.btnAllListOrLibraryCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sourec Site Name";
            // 
            // txtSiteName
            // 
            this.txtSiteName.Location = new System.Drawing.Point(202, 58);
            this.txtSiteName.Name = "txtSiteName";
            this.txtSiteName.Size = new System.Drawing.Size(210, 20);
            this.txtSiteName.TabIndex = 1;
            // 
            // lblListorLibrary
            // 
            this.lblListorLibrary.AutoSize = true;
            this.lblListorLibrary.Location = new System.Drawing.Point(62, 126);
            this.lblListorLibrary.Name = "lblListorLibrary";
            this.lblListorLibrary.Size = new System.Drawing.Size(71, 13);
            this.lblListorLibrary.TabIndex = 2;
            this.lblListorLibrary.Text = "List Or Library";
            // 
            // txtListName
            // 
            this.txtListName.Location = new System.Drawing.Point(202, 119);
            this.txtListName.Name = "txtListName";
            this.txtListName.Size = new System.Drawing.Size(210, 20);
            this.txtListName.TabIndex = 3;
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(62, 194);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(112, 13);
            this.lblDestination.TabIndex = 4;
            this.lblDestination.Text = "Destination Site Name";
            // 
            // txtDSSiteName
            // 
            this.txtDSSiteName.Location = new System.Drawing.Point(202, 191);
            this.txtDSSiteName.Name = "txtDSSiteName";
            this.txtDSSiteName.Size = new System.Drawing.Size(198, 20);
            this.txtDSSiteName.TabIndex = 5;
            // 
            // btnCreateEmptyList
            // 
            this.btnCreateEmptyList.Location = new System.Drawing.Point(65, 249);
            this.btnCreateEmptyList.Name = "btnCreateEmptyList";
            this.btnCreateEmptyList.Size = new System.Drawing.Size(150, 23);
            this.btnCreateEmptyList.TabIndex = 6;
            this.btnCreateEmptyList.Text = "Create Empty List/Library";
            this.btnCreateEmptyList.UseVisualStyleBackColor = true;
            this.btnCreateEmptyList.Click += new System.EventHandler(this.btnCreateEmptyList_Click);
            // 
            // btnCopyListOrLibraryData
            // 
            this.btnCopyListOrLibraryData.Location = new System.Drawing.Point(293, 249);
            this.btnCopyListOrLibraryData.Name = "btnCopyListOrLibraryData";
            this.btnCopyListOrLibraryData.Size = new System.Drawing.Size(160, 23);
            this.btnCopyListOrLibraryData.TabIndex = 7;
            this.btnCopyListOrLibraryData.Text = "Copy List/Library Data";
            this.btnCopyListOrLibraryData.UseVisualStyleBackColor = true;
            this.btnCopyListOrLibraryData.Click += new System.EventHandler(this.btnCopyListOrLibraryData_Click);
            // 
            // btnIncrementalCopy
            // 
            this.btnIncrementalCopy.Location = new System.Drawing.Point(65, 294);
            this.btnIncrementalCopy.Name = "btnIncrementalCopy";
            this.btnIncrementalCopy.Size = new System.Drawing.Size(150, 23);
            this.btnIncrementalCopy.TabIndex = 8;
            this.btnIncrementalCopy.Text = "Incremental Copy";
            this.btnIncrementalCopy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnIncrementalCopy.UseVisualStyleBackColor = true;
            this.btnIncrementalCopy.Click += new System.EventHandler(this.btnIncrementalCopy_Click);
            // 
            // btnAllListOrLibraryCopy
            // 
            this.btnAllListOrLibraryCopy.Location = new System.Drawing.Point(293, 294);
            this.btnAllListOrLibraryCopy.Name = "btnAllListOrLibraryCopy";
            this.btnAllListOrLibraryCopy.Size = new System.Drawing.Size(160, 23);
            this.btnAllListOrLibraryCopy.TabIndex = 9;
            this.btnAllListOrLibraryCopy.Text = "All ListOrLibrary ASIS";
            this.btnAllListOrLibraryCopy.UseVisualStyleBackColor = true;
            this.btnAllListOrLibraryCopy.Click += new System.EventHandler(this.btnAllListOrLibraryCopy_Click);
            // 
            // ManageListorLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 393);
            this.Controls.Add(this.btnAllListOrLibraryCopy);
            this.Controls.Add(this.btnIncrementalCopy);
            this.Controls.Add(this.btnCopyListOrLibraryData);
            this.Controls.Add(this.btnCreateEmptyList);
            this.Controls.Add(this.txtDSSiteName);
            this.Controls.Add(this.lblDestination);
            this.Controls.Add(this.txtListName);
            this.Controls.Add(this.lblListorLibrary);
            this.Controls.Add(this.txtSiteName);
            this.Controls.Add(this.label1);
            this.Name = "ManageListorLibrary";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSiteName;
        private System.Windows.Forms.Label lblListorLibrary;
        private System.Windows.Forms.TextBox txtListName;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.TextBox txtDSSiteName;
        private System.Windows.Forms.Button btnCreateEmptyList;
        private System.Windows.Forms.Button btnCopyListOrLibraryData;
        private System.Windows.Forms.Button btnIncrementalCopy;
        private System.Windows.Forms.Button btnAllListOrLibraryCopy;
    }
}

