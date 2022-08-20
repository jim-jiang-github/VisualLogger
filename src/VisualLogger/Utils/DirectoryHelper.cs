using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Utils
{
    internal class DirectoryHelper
    {
        public static bool ResetDirectory(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                else
                {
                    Directory.Delete(directory, true);
                    Directory.CreateDirectory(directory);
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                Log.Warning("Directory {directory} delete fail: {uae}", directory, uae);
                try
                {
                    var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                    }
                    Directory.Delete(directory, true);
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    Log.Fatal("Directory {directory} delete fail: {uae}", directory, ex);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Directory {directory} delete fail: {ex}", directory, ex);
                return false;
            }
            return true;
        }
    }
}
