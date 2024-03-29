﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// このソース コードは xsd によって自動生成されました。Version=4.8.3928.0 です。
// 
namespace AozoraEditor.Shared.Models.Project {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd", IsNullable=false)]
    public partial class Project {
        
        private ProjectEntry[] tocField;
        
        private File[] trashField;
        
        private ProjectNotes notesField;
        
        private ProjectSnippet snippetField;
        
        private ProjectTab[] tabsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Entry", IsNullable=false)]
        public ProjectEntry[] Toc {
            get {
                return this.tocField;
            }
            set {
                this.tocField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("File", IsNullable=false)]
        public File[] Trash {
            get {
                return this.trashField;
            }
            set {
                this.trashField = value;
            }
        }
        
        /// <remarks/>
        public ProjectNotes Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        public ProjectSnippet Snippet {
            get {
                return this.snippetField;
            }
            set {
                this.snippetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("tab", IsNullable=false)]
        public ProjectTab[] Tabs {
            get {
                return this.tabsField;
            }
            set {
                this.tabsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    public partial class ProjectEntry {
        
        private File itemField;
        
        private string titleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("File")]
        public File Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd", IsNullable=false)]
    public partial class File {
        
        private string pathField;
        
        private string typeField;
        
        private FileEncoding encodingField;
        
        private bool encodingFieldSpecified;
        
        public File() {
            this.typeField = "";
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("")]
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public FileEncoding encoding {
            get {
                return this.encodingField;
            }
            set {
                this.encodingField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool encodingSpecified {
            get {
                return this.encodingFieldSpecified;
            }
            set {
                this.encodingFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    public enum FileEncoding {
        
        /// <remarks/>
        Binary,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("UTF-8")]
        UTF8,
        
        /// <remarks/>
        Shift_JIS,
        
        /// <remarks/>
        NotSpecified,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    public partial class ProjectNotes {
        
        private Content itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Content")]
        public Content Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd", IsNullable=false)]
    public partial class Content {
        
        private object itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("File", typeof(File))]
        [System.Xml.Serialization.XmlElementAttribute("None", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("Text", typeof(ContentText))]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    public partial class ContentText {
        
        private string pathField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    public partial class ProjectSnippet {
        
        private Content itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Content")]
        public Content Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Project.xsd")]
    public partial class ProjectTab {
        
        private string typeField;
        
        private string argField;
        
        private int columnField;
        
        private bool columnFieldSpecified;
        
        private int rowField;
        
        private bool rowFieldSpecified;
        
        private string selectionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string arg {
            get {
                return this.argField;
            }
            set {
                this.argField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int column {
            get {
                return this.columnField;
            }
            set {
                this.columnField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool columnSpecified {
            get {
                return this.columnFieldSpecified;
            }
            set {
                this.columnFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int row {
            get {
                return this.rowField;
            }
            set {
                this.rowField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool rowSpecified {
            get {
                return this.rowFieldSpecified;
            }
            set {
                this.rowFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string selection {
            get {
                return this.selectionField;
            }
            set {
                this.selectionField = value;
            }
        }
    }
}
