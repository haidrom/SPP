﻿using System;
using System.Collections.Generic;
using System.IO;

namespace BookService.Classes
{
    class BookStorage
    {
        #region Fields

        private string path;

        #endregion

        #region Constructor

        public BookStorage(string filepath)
        {
            if(string.IsNullOrWhiteSpace(filepath)) throw new ArgumentNullException("Path cannot be null or empty.");
            path = string.Copy(filepath);
        }

        #endregion

        #region Public methods

        public void SaveBookList(IEnumerable<Book> books)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
                {
                    foreach (Book book in books)
                    {
                        SaveBook(book, writer);
                    }
                }
            }
            catch
            {
                throw new Exception("Saving error occured.");
            }
            
        }

        public IEnumerable<Book> LoadBookList()
        {
            if (!File.Exists(path)) throw new ArgumentException("Incorrect file path.");
            
            var books = new List<Book>();

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    try
                    {
                        books.Add(LoadBook(reader));
                    }
                    catch
                    {
                        throw new Exception("Loading error occured.");
                    }
                }
            }
            return books;
        }

        #endregion

        #region Private methods

        private void SaveBook (Book book, BinaryWriter binaryWriter)
        {
            try
            {
                binaryWriter.Write(book.ISBN);
                binaryWriter.Write(book.Author);
                binaryWriter.Write(book.Title);
                binaryWriter.Write(book.Publisher);
                binaryWriter.Write(book.PublishedAt);
                binaryWriter.Write(book.PagesCount);
                binaryWriter.Write(book.Price);
            }
            catch
            {
                throw new Exception();
            }
            
        }

        private Book LoadBook(BinaryReader binaryReader)
        {
            try
            {
                string isbn = binaryReader.ReadString();
                string author = binaryReader.ReadString();
                string title = binaryReader.ReadString();
                string publisher = binaryReader.ReadString();
                int publishedAt = binaryReader.ReadInt32();
                int pagesCount = binaryReader.ReadInt32();
                string price = binaryReader.ReadString();
                return new Book(isbn, author, title, publisher, publishedAt, pagesCount, price);
            }
            catch
            {
                throw new Exception();
            }
        }

        #endregion
    }
}
