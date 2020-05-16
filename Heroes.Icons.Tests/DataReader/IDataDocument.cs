﻿using System.Threading.Tasks;

namespace Heroes.Icons.Tests.DataReader
{
    public interface IDataDocument
    {
        void DataDocumentFileTest();

        void DataDocumentFileLocaleTest();

        void DataDocumentROMLocaleTest();

        void DataDocumentFileGSRTest();

        void DataDocumentROMGSRTest();

        void DataDocumentStreamTest();

        Task DataDocumentStreamAsyncTest();
    }
}