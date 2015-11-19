using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace Offline
{
    /// <summary>
    /// Class to help with IsoStorage use
    /// </summary>
    internal class IsoStorageUtil
    {
        /// <summary>
        /// Save data into file inside IsoStore if the file allready exists it will be overwritten
        /// </summary>
        /// <param name="fileName">Name of the target file</param>
        /// <param name="data">Content to be saved</param>
        /// <returns>Content saved</returns>
        internal static string SaveData(string fileName, string data)
        {
            using (IsolatedStorageFile isof = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isofs = new IsolatedStorageFileStream(fileName, FileMode.Create, isof))
                {
                    using (StreamWriter sw = new StreamWriter(isofs))
                    {
                        sw.Write(data);
                        sw.Close();
                        return data;
                    }
                }
            }
        }

        /// <summary>
        /// Load data of a file inside IsoStore if the file does not exist it returns String.Empty
        /// </summary>
        /// <param name="fileName">Name of the target file</param>
        /// <returns>Content loaded</returns>
        internal static string LoadData(string fileName)
        {
            string data = String.Empty;

            using (IsolatedStorageFile isof = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isof.FileExists(fileName))
                {
                    using (IsolatedStorageFileStream isofs = new IsolatedStorageFileStream(fileName, FileMode.Open, isof))
                    {
                        using (StreamReader sr = new StreamReader(isofs))
                        {
                            string lineOfData = String.Empty;
                            while ((lineOfData = sr.ReadLine()) != null)
                                data += lineOfData;
                        }
                    }
                    return data;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// Check whether or not the file exists inside the IsoStore
        /// </summary>
        /// <param name="fileName">Name of the target file</param>
        /// <returns><see cref="Boolean"/> whether or not the file exists</returns>
        internal static bool FileExists(string fileName)
        {
            using (IsolatedStorageFile isof = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isof.FileExists(fileName);
            }
        }
    }
}