using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace FullStackWork.Models
{
    public class ImageData
    {
        public string filepath;
        public string Name { get; set; }
        public ImageData(string path , string name)
        {
            filepath = path;
            Name = name;
        }
        //public Mat LoadImage()
        //{
        //    Mat img = new Mat(this.filepath);
        //    return img;
        //}
    }
}
