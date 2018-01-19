using System;
using System.Xml.Serialization;
using System.Collections.Generic;

    [XmlRoot(ElementName = "tagMenu")]
    public class TagMenu {
        [XmlElement(ElementName = "string")]
        public List<string> String { get; set; }
    }

    [XmlRoot(ElementName = "Item")]
    public class Item {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "prefabName")]
        public string PrefabName { get; set; }
        [XmlElement(ElementName = "category")]
        public string Category { get; set; }
        [XmlElement(ElementName = "amoutOfTags")]
        public int AmoutOfTags { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data {
        [XmlElement(ElementName = "Item")]
        public List<Item> Item { get; set; }
    }

    [XmlRoot(ElementName = "ItemData")]
    public class ItemData {
        [XmlElement(ElementName = "tagMenu")]
        public TagMenu TagMenu { get; set; }
        [XmlElement(ElementName = "data")]
        public Data Data { get; set; }
    }