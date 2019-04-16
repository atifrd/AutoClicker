using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clicker.DataSection
{
    class BusseinessMethods
    {
        /// <summary>
        /// موقوفوهایی که تصویر برای آپلود دارند
        /// </summary>
        /// <returns></returns>
        public static List<BarCodeData> ListForUpLoad()
       {
           try
           {
               using (var Context = new AutoClickDBEntities())
               {
                   // List<ImagesHistory> Query = Context.ImagesHistories.Where(s => s.ClassificationDateTime != null).ToList();

                   return Context.BarCodeDatas.Where(s => s.FilesINBarCodes.Any(p => p.UploadedDateTime == null)).ToList()
                       .Where(s=>s.کد_تفضیلی.ToString().StartsWith("1")).ToList();
                   //age goft oonayi ke ye khata nadashtan faghat =>p.errorr==null
               }
           }
           catch (Exception ex)
           {
               return null;
           }
          
       }
        /// <summary>
        ///  موقوفوهایی که تصویر برای آپلود دارند و در دسته بندی های مذکور هستند
        /// </summary>
        /// <returns></returns>
        public static List<double> ListForUpLoad_WithRelatedClass( List<int> Related_ClassesIds)
        {
            try
            {
                using (var Context = new AutoClickDBEntities())
                {
                 //List<double> a=   Context.FilesINBarCodes.Where(s => s.UploadedDateTime == null && Related_ClassesIds.Contains(s.FK_FileClass)).GroupBy(p=>p.FK_Barcode)
                   //     .Select
                     //   (p => p.Key).ToList();
                 return Context.FilesINBarCodes.Where(s => s.UploadedDateTime == null && Related_ClassesIds.Contains(s.FK_FileClass)).GroupBy(p => p.FK_Barcode)
                        .Select
                        (p => p.Key).ToList().Where(x=>x.ToString().StartsWith("1")).ToList();
                   
                    //age goft oonayi ke ye khata nadashtan faghat =>p.errorr==null
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        ///  رقبه هایی که تصویر برای آپلود دارند و در دسته بندی های مذکور هستند
        /// </summary>
        /// <returns></returns>
        public static List<double> GetListOfRaghabeForUpload_WithRelatedClass(List<int> Related_ClassesIds)
        {
            try
            {
                using (var Context = new AutoClickDBEntities())
                {//.BarCodeData.کد_تفضیلی
                    return Context.FilesINBarCodes.Where(s => s.UploadedDateTime == null && Related_ClassesIds.Contains(s.FK_FileClass))
                        .Select
                        (p => p.FK_Barcode).Distinct().ToList().Where(x => x.ToString().StartsWith("2")).ToList(); ;

                    //age goft oonayi ke ye khata nadashtan faghat =>p.errorr==null
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static List<FilesINBarCode> ImagesInEachCode(double Code)
        {
            try
            {
                using (var context = new AutoClickDBEntities())
                {
                    return context.FilesINBarCodes.Where(s => s.FK_Barcode== Code && s.UploadedDateTime == null).ToList();
                    //age goft oonayi ke ye khata nadashtan faghat =>p.errorr==null
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// chon too upload kole foldero yeho up mikone pas tedade folder(class)haye ye codo darmiarim
        /// albate oonbayi ke dar in masir baiad up shan
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="hj"></param>
        public static List<string> ClassesCountInEachCode(double Code, List<int> Related_ClassesIds)
        {
            try
            {
                using (var context = new AutoClickDBEntities())
                {
                   return context.FilesINBarCodes.Where(s => s.FK_Barcode == Code && s.UploadedDateTime == null
                          && Related_ClassesIds.Contains(s.FK_FileClass)
                          ).GroupBy(p => p.FileClass.FileClassNameEN).Select(c => c.Key).ToList();
                    
                        //.GroupBy(p=>p.FK_FileClass,n=> new { FileClassid=n.FK_FileClass}).Select(s=> new FilesINBarCode {FileClass=s.Key. })
                          //  .ToList();
                    //return context.FilesINBarCodes.Where(s => s.FK_Barcode == Code && s.UploadedDateTime == null).ToList();
                    //age goft oonayi ke ye khata nadashtan faghat =>p.errorr==null
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// تصاویر یک کد که در دسته بندی مورد نظر فرار گرفتند
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Related_ClassesIds"></param>
        /// <returns></returns>
        public static List<FilesINBarCode> ImagesInEachCodeWithRelatedClass(double Code, List<int> Related_ClassesIds)
        {
            try
            {
                using (var context = new AutoClickDBEntities())
                {
                    return context.FilesINBarCodes.Where(s => s.FK_Barcode == Code && s.UploadedDateTime == null
                           && Related_ClassesIds.Contains(s.FK_FileClass)
                           ).ToList();

                   
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        ///  بعداز زدن دکمه ثبت نهایی در صورت موفقیت آمیز بودن همه عکس های کد رو یه جا آپدیت می کنه
        /// </summary>
        public static void UpdateStateAfterSuccessfullUpload(double Code)
        {
            try
            {
                string NowDate = UiUtility.FarsiDate_ConvertDMYToYMD(BLLDate.getPersianDate(System.DateTime.Now)) + " " + System.DateTime.Now.TimeOfDay.ToString().Substring(0, 5);
                using (var context = new AutoClickDBEntities())
                {
                    ImagesHistory ImagesHistoryOb = new ImagesHistory();
                    ImagesHistoryOb.BarCodeData.کد_تفضیلی = Code;
                    ImagesHistoryOb.SuccessfulUploadDateTime = NowDate;
                    context.ImagesHistories.Add(ImagesHistoryOb);

                    List<FilesINBarCode> x = context.FilesINBarCodes.Where(s => s.FK_Barcode == Code && s.UploadedDateTime == null).ToList();
                    //age goft oonayi ke ye khata nadashtan faghat =>p.errorr==null
                    x.ForEach(s => { s.UploadedDateTime = NowDate; s.UploadedDateTime_G = System.DateTime.Now; });
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
            
        }
        /// <summary>
        /// بعد از زدن دکمه ثبت نهایی در صورت بروز خطا
        /// </summary>
        /// <param name="Code"></param>
        public static void UpdateStateAfterErrorAccurance(double Code, string GlobalErrorInUploadMessage, string ErrorMessage)
        {
            try
            {
                string NowDate = UiUtility.FarsiDate_ConvertDMYToYMD(BLLDate.getPersianDate(System.DateTime.Now));
                using (var context = new AutoClickDBEntities())
                {
                    ImagesHistory ImagesHistoryOb = new ImagesHistory();
                    ImagesHistoryOb.BarCodeData.کد_تفضیلی = Code;
                    ImagesHistoryOb.GlobalErrorInUploadDateTime = NowDate;
                    ImagesHistoryOb.GlobalErrorInUploadMessage = GlobalErrorInUploadMessage + " تاریخ: " + NowDate;
                    context.ImagesHistories.Add(ImagesHistoryOb);

                    List<FilesINBarCode> x = context.FilesINBarCodes.Where(s => s.FK_Barcode == Code && s.UploadedDateTime == null).ToList();
                    //age goft oonayi ke ye khata nadashtan faghat =>p.errorr==null
                    x.ForEach(s => s.ErrorMessage = ErrorMessage + " تاریخ: " + NowDate);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }           
        }
        /// <summary>
        /// بعداز با موفقیت آپلود شدن هر تصویر بانک رو آپلود می کنه
        /// </summary>
        /// <param name="Code"></param>
        public static void UpdateStateAfterSuccessfullUpload_eachImg(Int64 ImgId)
        {
            try
            {
                string NowDate = UiUtility.FarsiDate_ConvertDMYToYMD(BLLDate.getPersianDate(System.DateTime.Now)) + " " + System.DateTime.Now.TimeOfDay.ToString().Substring(0, 5);
                using (var context = new AutoClickDBEntities())
                {
                    FilesINBarCode x = context.FilesINBarCodes.Where(s => s.FileId == ImgId && s.UploadedDateTime == null).FirstOrDefault();
                    //age goft oonayi ke ye khata nadashtan faghat =>p.errorr==null
                    x.UploadedDateTime = NowDate;
                    x.UploadedDateTime_G = System.DateTime.Now;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }

        }
        /// <summary>
        /// رقبه هایی که تصویر برای آپلود دارند
        /// </summary>
        public static List<BarCodeData> GetListOfRaghabeForUpload()
        {
            try
            {
                using (var context=new AutoClickDBEntities())
                {
                    
                    return context.BarCodeDatas.Where(s=> s.FilesINBarCodes.Any(p => p.UploadedDateTime == null)).ToList()
                        .Where(s=>s.کد_تفضیلی.ToString().StartsWith("2")).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<FileClass> GetFileClassesList()
        {
            try
            {
                using (var context = new AutoClickDBEntities())
                {
                   return context.FileClasses.ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
                
            }
        }
        #region Reserve_Setting

        /// <summary>
        /// برای یک کلاینت یه تعداد تصویر رو رزرو می کنه
        /// </summary>
        /// <param name="Moghoofeh"></param>
        /// <param name="Related_ClassesIds"></param>
        /// <param name="ClientInfo"></param>
        /// <param name="cnt"></param>
        public static void Reserve_Client(bool Moghoofeh, List<int> Related_ClassesIds, string ClientInfo, int cnt)
        {
            try
            {
                string CodePre = Moghoofeh ? "1" : "2";
                string NowDate = UiUtility.FarsiDate_ConvertDMYToYMD(BLLDate.getPersianDate(System.DateTime.Now)) + " " + System.DateTime.Now.TimeOfDay.ToString().Substring(0, 5);
                using (var context = new AutoClickDBEntities())
                {
                   List<double> dd= context.FilesINBarCodes.Where(s => s.UploadedDateTime == null && Related_ClassesIds.Contains(s.FK_FileClass))
                        .Select
                        (p => p.FK_Barcode).Distinct().ToList().Where(x => x.ToString().StartsWith(CodePre)).Take(cnt).ToList();



                    List<FilesINBarCode> ReservedList = context.FilesINBarCodes.Where(s=> dd.Contains(s.FK_Barcode) &&    s.UploadedDateTime == null &&  Related_ClassesIds.Contains(s.FK_FileClass)).ToList();
                    ReservedList.ForEach(s => { s.ReservedDateTime_G = System.DateTime.Now; s.ReservedDateTime = NowDate; s.ReservedFor = ClientInfo; });                   
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }

        }
        /// <summary>
        /// در وقت توقف فرایند، تصاویر آپلود نشده ی یک کلاینت رو از حالت رزرو در میاره
        /// </summary>
        /// <param name="ClientInfo"></param>
        public static void UnReserve_Client(string ClientInfo)
        {
            try
            {                
                using (var context = new AutoClickDBEntities())
                {
                    List<FilesINBarCode> ReservedList = context.FilesINBarCodes.Where(s => s.ReservedFor==ClientInfo && s.UploadedDateTime==null).ToList();
                    
                    ReservedList.ForEach(s => { s.ReservedDateTime_G = null; s.ReservedDateTime = null; s.ReservedFor = null; });
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }

        }
        #endregion

    }
}
