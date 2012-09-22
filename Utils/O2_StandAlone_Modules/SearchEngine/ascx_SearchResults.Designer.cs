// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2File:ascx_SearchResults.Controllers.cs
//O2File:ascx_SearchResults.cs 

namespace O2.Tool.SearchEngine.Ascx
{
    partial class ascx_SearchResults
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.llClearAll = new System.Windows.Forms.LinkLabel();
            this.llClearSelected = new System.Windows.Forms.LinkLabel();
            this.llRunMultipleSearches = new System.Windows.Forms.LinkLabel();
            this.dgvSearchCriteria = new System.Windows.Forms.DataGridView();
            this.SearchCriteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NegativeSearch = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tbSingleSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.llRefreshSearchResultsView = new System.Windows.Forms.LinkLabel();
            this.tcSearchResults = new System.Windows.Forms.TabControl();
            this.tpDataGridView = new System.Windows.Forms.TabPage();
            this.dgvSearchResults = new System.Windows.Forms.DataGridView();
            this.tpTreeView = new System.Windows.Forms.TabPage();
            this.tbTreeView_FilterText4 = new System.Windows.Forms.TextBox();
            this.tbTreeView_FilterText3 = new System.Windows.Forms.TextBox();
            this.tbTreeView_FilterText2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTreeView_FilterText1 = new System.Windows.Forms.TextBox();
            this.cbTreeView_FilterType4 = new System.Windows.Forms.ComboBox();
            this.cbTreeView_FilterType3 = new System.Windows.Forms.ComboBox();
            this.cbTreeView_FilterType2 = new System.Windows.Forms.ComboBox();
            this.cbTreeView_FilterType1 = new System.Windows.Forms.ComboBox();
            this.tvSearchResults = new System.Windows.Forms.TreeView();
            this.tpTextView = new System.Windows.Forms.TabPage();
            this.tbSearchResults = new System.Windows.Forms.TextBox();
            this.lbSearchResultsStats = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbOpenSelectedItemInMainGUIWindow = new System.Windows.Forms.CheckBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchCriteria)).BeginInit();
            this.tcSearchResults.SuspendLayout();
            this.tpDataGridView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchResults)).BeginInit();
            this.tpTreeView.SuspendLayout();
            this.tpTextView.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.llClearAll);
            this.splitContainer1.Panel1.Controls.Add(this.llClearSelected);
            this.splitContainer1.Panel1.Controls.Add(this.llRunMultipleSearches);
            this.splitContainer1.Panel1.Controls.Add(this.dgvSearchCriteria);
            this.splitContainer1.Panel1.Controls.Add(this.tbSingleSearch);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cbOpenSelectedItemInMainGUIWindow);
            this.splitContainer1.Panel2.Controls.Add(this.llRefreshSearchResultsView);
            this.splitContainer1.Panel2.Controls.Add(this.tcSearchResults);
            this.splitContainer1.Panel2.Controls.Add(this.lbSearchResultsStats);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Size = new System.Drawing.Size(720, 372);
            this.splitContainer1.SplitterDistance = 172;
            this.splitContainer1.TabIndex = 0;
            // 
            // llClearAll
            // 
            this.llClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.llClearAll.AutoSize = true;
            this.llClearAll.Location = new System.Drawing.Point(122, 354);
            this.llClearAll.Name = "llClearAll";
            this.llClearAll.Size = new System.Drawing.Size(43, 13);
            this.llClearAll.TabIndex = 74;
            this.llClearAll.TabStop = true;
            this.llClearAll.Text = "clear all";
            // 
            // llClearSelected
            // 
            this.llClearSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.llClearSelected.AutoSize = true;
            this.llClearSelected.Location = new System.Drawing.Point(0, 354);
            this.llClearSelected.Name = "llClearSelected";
            this.llClearSelected.Size = new System.Drawing.Size(73, 13);
            this.llClearSelected.TabIndex = 73;
            this.llClearSelected.TabStop = true;
            this.llClearSelected.Text = "clear selected";
            this.llClearSelected.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llClearSelected_LinkClicked);
            // 
            // llRunMultipleSearches
            // 
            this.llRunMultipleSearches.AutoSize = true;
            this.llRunMultipleSearches.Location = new System.Drawing.Point(109, 46);
            this.llRunMultipleSearches.Name = "llRunMultipleSearches";
            this.llRunMultipleSearches.Size = new System.Drawing.Size(57, 13);
            this.llRunMultipleSearches.TabIndex = 72;
            this.llRunMultipleSearches.TabStop = true;
            this.llRunMultipleSearches.Text = "run search";
            this.llRunMultipleSearches.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llRunMultipleSearches_LinkClicked);
            // 
            // dgvSearchCriteria
            // 
            this.dgvSearchCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSearchCriteria.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSearchCriteria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchCriteria.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SearchCriteria,
            this.NegativeSearch,
            this.enabled});
            this.dgvSearchCriteria.Location = new System.Drawing.Point(1, 62);
            this.dgvSearchCriteria.Name = "dgvSearchCriteria";
            this.dgvSearchCriteria.RowHeadersWidth = 10;
            this.dgvSearchCriteria.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSearchCriteria.Size = new System.Drawing.Size(164, 290);
            this.dgvSearchCriteria.TabIndex = 1;
            this.dgvSearchCriteria.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchCriteria_CellValueChanged);
            this.dgvSearchCriteria.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvSearchCriteria_RowsAdded);
            // 
            // SearchCriteria
            // 
            this.SearchCriteria.HeaderText = "Criteria";
            this.SearchCriteria.Name = "SearchCriteria";
            // 
            // NegativeSearch
            // 
            this.NegativeSearch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.NegativeSearch.HeaderText = "!";
            this.NegativeSearch.MinimumWidth = 20;
            this.NegativeSearch.Name = "NegativeSearch";
            this.NegativeSearch.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.NegativeSearch.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.NegativeSearch.Width = 20;
            // 
            // enabled
            // 
            this.enabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.enabled.HeaderText = "ON";
            this.enabled.MinimumWidth = 25;
            this.enabled.Name = "enabled";
            this.enabled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.enabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.enabled.Width = 25;
            // 
            // tbSingleSearch
            // 
            this.tbSingleSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSingleSearch.Location = new System.Drawing.Point(1, 20);
            this.tbSingleSearch.Name = "tbSingleSearch";
            this.tbSingleSearch.Size = new System.Drawing.Size(164, 20);
            this.tbSingleSearch.TabIndex = 71;
            this.tbSingleSearch.TextChanged += new System.EventHandler(this.tbSingleSearch_TextChanged);
            this.tbSingleSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSingleSearch_KeyDown);
            this.tbSingleSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSingleSearch_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 70;
            this.label2.Text = "Single Search";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(-1, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 68;
            this.label1.Text = "Multiple Searches";
            // 
            // llRefreshSearchResultsView
            // 
            this.llRefreshSearchResultsView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llRefreshSearchResultsView.AutoSize = true;
            this.llRefreshSearchResultsView.Location = new System.Drawing.Point(473, 2);
            this.llRefreshSearchResultsView.Name = "llRefreshSearchResultsView";
            this.llRefreshSearchResultsView.Size = new System.Drawing.Size(64, 13);
            this.llRefreshSearchResultsView.TabIndex = 75;
            this.llRefreshSearchResultsView.TabStop = true;
            this.llRefreshSearchResultsView.Text = "refresh view";
            this.llRefreshSearchResultsView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llRefreshSearchResultsView_LinkClicked);
            // 
            // tcSearchResults
            // 
            this.tcSearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcSearchResults.Controls.Add(this.tpDataGridView);
            this.tcSearchResults.Controls.Add(this.tpTreeView);
            this.tcSearchResults.Controls.Add(this.tpTextView);
            this.tcSearchResults.Location = new System.Drawing.Point(3, 20);
            this.tcSearchResults.Name = "tcSearchResults";
            this.tcSearchResults.SelectedIndex = 0;
            this.tcSearchResults.Size = new System.Drawing.Size(534, 324);
            this.tcSearchResults.TabIndex = 69;
            this.tcSearchResults.SelectedIndexChanged += new System.EventHandler(this.tcSearchResults_SelectedIndexChanged);
            // 
            // tpDataGridView
            // 
            this.tpDataGridView.Controls.Add(this.dgvSearchResults);
            this.tpDataGridView.Location = new System.Drawing.Point(4, 22);
            this.tpDataGridView.Name = "tpDataGridView";
            this.tpDataGridView.Padding = new System.Windows.Forms.Padding(3);
            this.tpDataGridView.Size = new System.Drawing.Size(526, 298);
            this.tpDataGridView.TabIndex = 0;
            this.tpDataGridView.Text = "Data Grid View";
            this.tpDataGridView.UseVisualStyleBackColor = true;
            // 
            // dgvSearchResults
            // 
            this.dgvSearchResults.AllowUserToAddRows = false;
            this.dgvSearchResults.AllowUserToDeleteRows = false;
            this.dgvSearchResults.AllowUserToOrderColumns = true;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvSearchResults.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSearchResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSearchResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSearchResults.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSearchResults.Location = new System.Drawing.Point(3, 3);
            this.dgvSearchResults.Name = "dgvSearchResults";
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvSearchResults.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSearchResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSearchResults.Size = new System.Drawing.Size(520, 292);
            this.dgvSearchResults.TabIndex = 67;
            this.dgvSearchResults.SelectionChanged += new System.EventHandler(this.dgvSearchResults_SelectionChanged);
            this.dgvSearchResults.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchResults_CellContentClick);
            // 
            // tpTreeView
            // 
            this.tpTreeView.Controls.Add(this.tbTreeView_FilterText4);
            this.tpTreeView.Controls.Add(this.tbTreeView_FilterText3);
            this.tpTreeView.Controls.Add(this.tbTreeView_FilterText2);
            this.tpTreeView.Controls.Add(this.label4);
            this.tpTreeView.Controls.Add(this.label3);
            this.tpTreeView.Controls.Add(this.tbTreeView_FilterText1);
            this.tpTreeView.Controls.Add(this.cbTreeView_FilterType4);
            this.tpTreeView.Controls.Add(this.cbTreeView_FilterType3);
            this.tpTreeView.Controls.Add(this.cbTreeView_FilterType2);
            this.tpTreeView.Controls.Add(this.cbTreeView_FilterType1);
            this.tpTreeView.Controls.Add(this.tvSearchResults);
            this.tpTreeView.Location = new System.Drawing.Point(4, 22);
            this.tpTreeView.Name = "tpTreeView";
            this.tpTreeView.Padding = new System.Windows.Forms.Padding(3);
            this.tpTreeView.Size = new System.Drawing.Size(526, 302);
            this.tpTreeView.TabIndex = 1;
            this.tpTreeView.Text = "TreeView (not fully implemented)";
            this.tpTreeView.UseVisualStyleBackColor = true;
            // 
            // tbTreeView_FilterText4
            // 
            this.tbTreeView_FilterText4.Enabled = false;
            this.tbTreeView_FilterText4.Location = new System.Drawing.Point(389, 29);
            this.tbTreeView_FilterText4.Name = "tbTreeView_FilterText4";
            this.tbTreeView_FilterText4.Size = new System.Drawing.Size(66, 20);
            this.tbTreeView_FilterText4.TabIndex = 11;
            // 
            // tbTreeView_FilterText3
            // 
            this.tbTreeView_FilterText3.Enabled = false;
            this.tbTreeView_FilterText3.Location = new System.Drawing.Point(286, 29);
            this.tbTreeView_FilterText3.Name = "tbTreeView_FilterText3";
            this.tbTreeView_FilterText3.Size = new System.Drawing.Size(66, 20);
            this.tbTreeView_FilterText3.TabIndex = 10;
            // 
            // tbTreeView_FilterText2
            // 
            this.tbTreeView_FilterText2.Enabled = false;
            this.tbTreeView_FilterText2.Location = new System.Drawing.Point(179, 29);
            this.tbTreeView_FilterText2.Name = "tbTreeView_FilterText2";
            this.tbTreeView_FilterText2.Size = new System.Drawing.Size(66, 20);
            this.tbTreeView_FilterText2.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "filter text";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "filter type";
            // 
            // tbTreeView_FilterText1
            // 
            this.tbTreeView_FilterText1.Enabled = false;
            this.tbTreeView_FilterText1.Location = new System.Drawing.Point(75, 29);
            this.tbTreeView_FilterText1.Name = "tbTreeView_FilterText1";
            this.tbTreeView_FilterText1.Size = new System.Drawing.Size(66, 20);
            this.tbTreeView_FilterText1.TabIndex = 5;
            // 
            // cbTreeView_FilterType4
            // 
            this.cbTreeView_FilterType4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTreeView_FilterType4.FormattingEnabled = true;
            this.cbTreeView_FilterType4.Location = new System.Drawing.Point(389, 6);
            this.cbTreeView_FilterType4.Name = "cbTreeView_FilterType4";
            this.cbTreeView_FilterType4.Size = new System.Drawing.Size(100, 21);
            this.cbTreeView_FilterType4.TabIndex = 4;
            // 
            // cbTreeView_FilterType3
            // 
            this.cbTreeView_FilterType3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTreeView_FilterType3.FormattingEnabled = true;
            this.cbTreeView_FilterType3.Location = new System.Drawing.Point(286, 6);
            this.cbTreeView_FilterType3.Name = "cbTreeView_FilterType3";
            this.cbTreeView_FilterType3.Size = new System.Drawing.Size(97, 21);
            this.cbTreeView_FilterType3.TabIndex = 3;
            // 
            // cbTreeView_FilterType2
            // 
            this.cbTreeView_FilterType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTreeView_FilterType2.FormattingEnabled = true;
            this.cbTreeView_FilterType2.Location = new System.Drawing.Point(179, 6);
            this.cbTreeView_FilterType2.Name = "cbTreeView_FilterType2";
            this.cbTreeView_FilterType2.Size = new System.Drawing.Size(101, 21);
            this.cbTreeView_FilterType2.TabIndex = 2;
            // 
            // cbTreeView_FilterType1
            // 
            this.cbTreeView_FilterType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTreeView_FilterType1.FormattingEnabled = true;
            this.cbTreeView_FilterType1.Location = new System.Drawing.Point(75, 6);
            this.cbTreeView_FilterType1.Name = "cbTreeView_FilterType1";
            this.cbTreeView_FilterType1.Size = new System.Drawing.Size(98, 21);
            this.cbTreeView_FilterType1.TabIndex = 1;
            // 
            // tvSearchResults
            // 
            this.tvSearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvSearchResults.Location = new System.Drawing.Point(6, 50);
            this.tvSearchResults.Name = "tvSearchResults";
            this.tvSearchResults.Size = new System.Drawing.Size(517, 244);
            this.tvSearchResults.TabIndex = 0;
            this.tvSearchResults.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvSearchResults_BeforeExpand);
            // 
            // tpTextView
            // 
            this.tpTextView.Controls.Add(this.tbSearchResults);
            this.tpTextView.Location = new System.Drawing.Point(4, 22);
            this.tpTextView.Name = "tpTextView";
            this.tpTextView.Padding = new System.Windows.Forms.Padding(3);
            this.tpTextView.Size = new System.Drawing.Size(526, 302);
            this.tpTextView.TabIndex = 2;
            this.tpTextView.Text = "Text View (CVS)  (not fully implemented)";
            this.tpTextView.UseVisualStyleBackColor = true;
            // 
            // tbSearchResults
            // 
            this.tbSearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSearchResults.Location = new System.Drawing.Point(3, 3);
            this.tbSearchResults.Multiline = true;
            this.tbSearchResults.Name = "tbSearchResults";
            this.tbSearchResults.Size = new System.Drawing.Size(520, 296);
            this.tbSearchResults.TabIndex = 0;
            // 
            // lbSearchResultsStats
            // 
            this.lbSearchResultsStats.AutoSize = true;
            this.lbSearchResultsStats.Location = new System.Drawing.Point(99, 2);
            this.lbSearchResultsStats.Name = "lbSearchResultsStats";
            this.lbSearchResultsStats.Size = new System.Drawing.Size(16, 13);
            this.lbSearchResultsStats.TabIndex = 68;
            this.lbSearchResultsStats.Text = "...";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(0, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 66;
            this.label5.Text = "Search Results:";
            // 
            // cbOpenSelectedItemInMainGUIWindow
            // 
            this.cbOpenSelectedItemInMainGUIWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbOpenSelectedItemInMainGUIWindow.AutoSize = true;
            this.cbOpenSelectedItemInMainGUIWindow.Checked = true;
            this.cbOpenSelectedItemInMainGUIWindow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOpenSelectedItemInMainGUIWindow.Location = new System.Drawing.Point(3, 350);
            this.cbOpenSelectedItemInMainGUIWindow.Name = "cbOpenSelectedItemInMainGUIWindow";
            this.cbOpenSelectedItemInMainGUIWindow.Size = new System.Drawing.Size(214, 17);
            this.cbOpenSelectedItemInMainGUIWindow.TabIndex = 76;
            this.cbOpenSelectedItemInMainGUIWindow.Text = "Open selected item in main GUI window";
            this.cbOpenSelectedItemInMainGUIWindow.UseVisualStyleBackColor = true;
            // 
            // ascx_SearchResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ascx_SearchResults";
            this.Size = new System.Drawing.Size(720, 372);
            this.Load += new System.EventHandler(this.ascx_SearchResults_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchCriteria)).EndInit();
            this.tcSearchResults.ResumeLayout(false);
            this.tpDataGridView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchResults)).EndInit();
            this.tpTreeView.ResumeLayout(false);
            this.tpTreeView.PerformLayout();
            this.tpTextView.ResumeLayout(false);
            this.tpTextView.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvSearchCriteria;
        private System.Windows.Forms.DataGridViewTextBoxColumn SearchCriteria;
        private System.Windows.Forms.DataGridViewCheckBoxColumn NegativeSearch;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabled;
        private System.Windows.Forms.DataGridView dgvSearchResults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbSearchResultsStats;
        private System.Windows.Forms.TextBox tbSingleSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel llClearAll;
        private System.Windows.Forms.LinkLabel llClearSelected;
        private System.Windows.Forms.LinkLabel llRunMultipleSearches;
        private System.Windows.Forms.TabControl tcSearchResults;
        private System.Windows.Forms.TabPage tpDataGridView;
        private System.Windows.Forms.TabPage tpTreeView;
        private System.Windows.Forms.LinkLabel llRefreshSearchResultsView;
        private System.Windows.Forms.TreeView tvSearchResults;
        private System.Windows.Forms.TabPage tpTextView;
        private System.Windows.Forms.TextBox tbSearchResults;
        private System.Windows.Forms.TextBox tbTreeView_FilterText1;
        private System.Windows.Forms.ComboBox cbTreeView_FilterType4;
        private System.Windows.Forms.ComboBox cbTreeView_FilterType3;
        private System.Windows.Forms.ComboBox cbTreeView_FilterType2;
        private System.Windows.Forms.ComboBox cbTreeView_FilterType1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTreeView_FilterText4;
        private System.Windows.Forms.TextBox tbTreeView_FilterText3;
        private System.Windows.Forms.TextBox tbTreeView_FilterText2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbOpenSelectedItemInMainGUIWindow;

    }
}
