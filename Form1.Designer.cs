namespace CodeGeneration
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTargetFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTargetFolder = new System.Windows.Forms.TextBox();
            this.btnEnumValues = new System.Windows.Forms.Button();
            this.btnSchemaFile = new System.Windows.Forms.Button();
            this.txtSchemaFile = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDBSP = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDbUtility = new System.Windows.Forms.Button();
            this.txtDbUtility = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSPTargetFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDbTargetFolder = new System.Windows.Forms.TextBox();
            this.btnDbSubmission = new System.Windows.Forms.Button();
            this.txtDbSubmission = new System.Windows.Forms.TextBox();
            this.btnRaml = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRaml);
            this.groupBox1.Controls.Add(this.btnTargetFolder);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtTargetFolder);
            this.groupBox1.Controls.Add(this.btnEnumValues);
            this.groupBox1.Controls.Add(this.btnSchemaFile);
            this.groupBox1.Controls.Add(this.txtSchemaFile);
            this.groupBox1.Location = new System.Drawing.Point(18, 27);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(382, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FN Schema";
            // 
            // btnTargetFolder
            // 
            this.btnTargetFolder.Location = new System.Drawing.Point(338, 58);
            this.btnTargetFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnTargetFolder.Name = "btnTargetFolder";
            this.btnTargetFolder.Size = new System.Drawing.Size(30, 19);
            this.btnTargetFolder.TabIndex = 5;
            this.btnTargetFolder.Text = "...";
            this.btnTargetFolder.UseVisualStyleBackColor = true;
            this.btnTargetFolder.Click += new System.EventHandler(this.btnTargetFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Target Folder";
            // 
            // txtTargetFolder
            // 
            this.txtTargetFolder.Location = new System.Drawing.Point(19, 58);
            this.txtTargetFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtTargetFolder.Name = "txtTargetFolder";
            this.txtTargetFolder.Size = new System.Drawing.Size(315, 20);
            this.txtTargetFolder.TabIndex = 3;
            // 
            // btnEnumValues
            // 
            this.btnEnumValues.Location = new System.Drawing.Point(244, 92);
            this.btnEnumValues.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnEnumValues.Name = "btnEnumValues";
            this.btnEnumValues.Size = new System.Drawing.Size(124, 21);
            this.btnEnumValues.TabIndex = 2;
            this.btnEnumValues.Text = "Enum Values";
            this.toolTip1.SetToolTip(this.btnEnumValues, "Generate C# enum type and SQL Insert statements for all Enum Code fields");
            this.btnEnumValues.UseVisualStyleBackColor = true;
            this.btnEnumValues.Click += new System.EventHandler(this.btnEnumValues_Click);
            // 
            // btnSchemaFile
            // 
            this.btnSchemaFile.Location = new System.Drawing.Point(338, 17);
            this.btnSchemaFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSchemaFile.Name = "btnSchemaFile";
            this.btnSchemaFile.Size = new System.Drawing.Size(30, 19);
            this.btnSchemaFile.TabIndex = 1;
            this.btnSchemaFile.Text = "...";
            this.btnSchemaFile.UseVisualStyleBackColor = true;
            this.btnSchemaFile.Click += new System.EventHandler(this.btnSchemaFile_Click);
            // 
            // txtSchemaFile
            // 
            this.txtSchemaFile.Location = new System.Drawing.Point(19, 17);
            this.txtSchemaFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtSchemaFile.Name = "txtSchemaFile";
            this.txtSchemaFile.Size = new System.Drawing.Size(315, 20);
            this.txtSchemaFile.TabIndex = 0;
            // 
            // btnDBSP
            // 
            this.btnDBSP.Location = new System.Drawing.Point(244, 197);
            this.btnDBSP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDBSP.Name = "btnDBSP";
            this.btnDBSP.Size = new System.Drawing.Size(124, 21);
            this.btnDBSP.TabIndex = 2;
            this.btnDBSP.Text = "SP and SP Call";
            this.toolTip1.SetToolTip(this.btnDBSP, "Generate SP SQL Scripts and C# Call to SP from DB Schema");
            this.btnDBSP.UseVisualStyleBackColor = true;
            this.btnDBSP.Click += new System.EventHandler(this.btnDBSP_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btnDbUtility);
            this.groupBox2.Controls.Add(this.txtDbUtility);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnSPTargetFolder);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtDbTargetFolder);
            this.groupBox2.Controls.Add(this.btnDBSP);
            this.groupBox2.Controls.Add(this.btnDbSubmission);
            this.groupBox2.Controls.Add(this.txtDbSubmission);
            this.groupBox2.Location = new System.Drawing.Point(18, 171);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(382, 223);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "FN DB Schema";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 57);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Utility DB Schema";
            // 
            // btnDbUtility
            // 
            this.btnDbUtility.Location = new System.Drawing.Point(338, 75);
            this.btnDbUtility.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDbUtility.Name = "btnDbUtility";
            this.btnDbUtility.Size = new System.Drawing.Size(30, 19);
            this.btnDbUtility.TabIndex = 8;
            this.btnDbUtility.Text = "...";
            this.btnDbUtility.UseVisualStyleBackColor = true;
            this.btnDbUtility.Click += new System.EventHandler(this.btnDbUtility_Click);
            // 
            // txtDbUtility
            // 
            this.txtDbUtility.Location = new System.Drawing.Point(19, 76);
            this.txtDbUtility.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtDbUtility.Name = "txtDbUtility";
            this.txtDbUtility.Size = new System.Drawing.Size(315, 20);
            this.txtDbUtility.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 15);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Submission DB Schema";
            // 
            // btnSPTargetFolder
            // 
            this.btnSPTargetFolder.Location = new System.Drawing.Point(338, 136);
            this.btnSPTargetFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSPTargetFolder.Name = "btnSPTargetFolder";
            this.btnSPTargetFolder.Size = new System.Drawing.Size(30, 19);
            this.btnSPTargetFolder.TabIndex = 5;
            this.btnSPTargetFolder.Text = "...";
            this.btnSPTargetFolder.UseVisualStyleBackColor = true;
            this.btnSPTargetFolder.Click += new System.EventHandler(this.btnSPTargetFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 120);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Target Folder";
            // 
            // txtDbTargetFolder
            // 
            this.txtDbTargetFolder.Location = new System.Drawing.Point(19, 136);
            this.txtDbTargetFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtDbTargetFolder.Name = "txtDbTargetFolder";
            this.txtDbTargetFolder.Size = new System.Drawing.Size(315, 20);
            this.txtDbTargetFolder.TabIndex = 3;
            // 
            // btnDbSubmission
            // 
            this.btnDbSubmission.Location = new System.Drawing.Point(338, 32);
            this.btnDbSubmission.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDbSubmission.Name = "btnDbSubmission";
            this.btnDbSubmission.Size = new System.Drawing.Size(30, 19);
            this.btnDbSubmission.TabIndex = 1;
            this.btnDbSubmission.Text = "...";
            this.btnDbSubmission.UseVisualStyleBackColor = true;
            this.btnDbSubmission.Click += new System.EventHandler(this.btnDbSubmission_Click);
            // 
            // txtDbSubmission
            // 
            this.txtDbSubmission.Location = new System.Drawing.Point(19, 33);
            this.txtDbSubmission.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtDbSubmission.Name = "txtDbSubmission";
            this.txtDbSubmission.Size = new System.Drawing.Size(315, 20);
            this.txtDbSubmission.TabIndex = 0;
            // 
            // btnRaml
            // 
            this.btnRaml.Location = new System.Drawing.Point(116, 92);
            this.btnRaml.Margin = new System.Windows.Forms.Padding(2);
            this.btnRaml.Name = "btnRaml";
            this.btnRaml.Size = new System.Drawing.Size(124, 21);
            this.btnRaml.TabIndex = 6;
            this.btnRaml.Text = "Raml";
            this.toolTip1.SetToolTip(this.btnRaml, "Generate C# enum type and SQL Insert statements for all Enum Code fields");
            this.btnRaml.UseVisualStyleBackColor = true;
            this.btnRaml.Click += new System.EventHandler(this.btnRaml_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 415);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSchemaFile;
        private System.Windows.Forms.TextBox txtSchemaFile;
        private System.Windows.Forms.Button btnEnumValues;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnTargetFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTargetFolder;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSPTargetFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDbTargetFolder;
        private System.Windows.Forms.Button btnDBSP;
        private System.Windows.Forms.Button btnDbSubmission;
        private System.Windows.Forms.TextBox txtDbSubmission;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDbUtility;
        private System.Windows.Forms.TextBox txtDbUtility;
        private System.Windows.Forms.Button btnRaml;
    }
}

