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
namespace AozoraEditor.Shared.Models.Notes {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Notes.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Notes.xsd", IsNullable=false)]
    public partial class notes {
        
        private object[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tasks", typeof(notesTasks))]
        [System.Xml.Serialization.XmlElementAttribute("text", typeof(string))]
        public object[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Notes.xsd")]
    public partial class notesTasks {
        
        private task[] itemsField;
        
        private string headerField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("task")]
        public task[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string header {
            get {
                return this.headerField;
            }
            set {
                this.headerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Notes.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraE" +
        "ditorSharedUI/Models/Notes.xsd", IsNullable=false)]
    public partial class task {
        
        private task[] itemsField;
        
        private string headerField;
        
        private bool isCheckedField;
        
        private bool isCheckedFieldSpecified;
        
        private System.DateTime deadlineField;
        
        private bool deadlineFieldSpecified;
        
        private string intervalField;
        
        private bool staredField;
        
        private bool staredFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("task")]
        public task[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string header {
            get {
                return this.headerField;
            }
            set {
                this.headerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isChecked {
            get {
                return this.isCheckedField;
            }
            set {
                this.isCheckedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isCheckedSpecified {
            get {
                return this.isCheckedFieldSpecified;
            }
            set {
                this.isCheckedFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime deadline {
            get {
                return this.deadlineField;
            }
            set {
                this.deadlineField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool deadlineSpecified {
            get {
                return this.deadlineFieldSpecified;
            }
            set {
                this.deadlineFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="duration")]
        public string interval {
            get {
                return this.intervalField;
            }
            set {
                this.intervalField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool stared {
            get {
                return this.staredField;
            }
            set {
                this.staredField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool staredSpecified {
            get {
                return this.staredFieldSpecified;
            }
            set {
                this.staredFieldSpecified = value;
            }
        }
    }
}
