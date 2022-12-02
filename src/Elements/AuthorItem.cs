﻿namespace FB2Library.HeaderItems
{
    class AuthorItem : AuthorType
    {
        public AuthorItem()
        {
            ElementName = AuthorElementName;
        }
    }

    class TranslatorItem : AuthorType
    {
        public TranslatorItem()
        {
            ElementName = TranslatorElementName;
        }
    }

    class PublisherItem : AuthorType
    {
        public PublisherItem()
        {
            ElementName = PublisherElementName;
        }
    }
}
