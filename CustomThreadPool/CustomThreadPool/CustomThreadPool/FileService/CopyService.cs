﻿using System;
using System.IO;
using System.Threading;
using CustomThreadPool.CustomPool;

namespace CustomThreadPool.FileService
{
    class CopyService
    {
        #region Private Fields

        private TaskQueue _taskQueue;
        private int _filesCopied = 0;
        private int _copyErrorsOccured = 0;
        private string _src;
        private string _dest;

        #endregion

        #region Construtor

        public CopyService(string src, string dest, int threadsCount)
        {
            _src = src;
            _dest = dest;
            if (_src.CompareTo(_dest) == 0) throw new ArgumentException("Sorce directory cannot be equal to destination directory.");
            if (!Directory.Exists(_src)) throw new ArgumentException("Wrong source directory.");
            if (!Directory.Exists(_dest))
            {
                try
                {
                    Directory.CreateDirectory(_dest);
                }
                catch
                {
                    throw new Exception("Cannot create destination directory.");
                }
            }
            if (threadsCount < 2)
            {
                Console.WriteLine("Created thread pool with default count of threads(5).");
                _taskQueue = new TaskQueue();
            }
            else _taskQueue = new TaskQueue(threadsCount);
        }

        #endregion

        #region Public Methods

        public void StartCopy()
        {
            foreach (var dir in Directory.GetDirectories(_src, "*", SearchOption.AllDirectories))
            {
                string newDir = dir.Replace(_src, _dest);
                try
                {
                    Directory.CreateDirectory(newDir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Creating directory error ({ex.Message}).");
                }
            }
            foreach (var file in Directory.GetFiles(_src, "*.*", SearchOption.AllDirectories))
            {
                string newFile = file.Replace(_src, _dest);
                _taskQueue.EnqueueTask(delegate {
                    try
                    {
                        File.Copy(file, newFile, true);
                        Console.WriteLine($"File copied:\n   from {file}\n   to {newFile}");
                        Interlocked.Increment(ref _filesCopied);               
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Copying file error ({ex.Message}):\n   from {file}\n   to {newFile}");
                        Interlocked.Increment(ref _copyErrorsOccured);
                    }
                });
            }
            _taskQueue.StopWorking();
            Console.WriteLine($"Files copied: {_filesCopied}\nErrors occured: {_copyErrorsOccured}");
        }

        #endregion
    }
}
