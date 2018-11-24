using System;
using Cafocha.Repository.DAL;
using PdfRpt.Core.Contracts;


namespace Cafocha.GUI.Helper.PrintHelper.Report
{
    

    public interface IListPdfReport
    {
        IPdfReportData CreatePdfReport(AdminwsOfCloudPOS unitofwork, DateTime startime, DateTime endTime, string folderName);

        IPdfReportData CreateDetailsPdfReport(AdminwsOfCloudPOS unitofwork, DateTime startime, DateTime endTime, string folderName);

        IPdfReportData CreateEntityPdfReport(AdminwsOfCloudPOS unitofwork, DateTime startime, DateTime endTime, string folderName);

        IPdfReportData CreateMonthPdfReport(AdminwsOfCloudPOS unitofwork, string folderName);

        IPdfReportData CreateDayPdfReport(AdminwsOfCloudPOS unitofwork, string folderName);

        IPdfReportData CreateYearPdfReport(AdminwsOfCloudPOS unitofwork, string folderName);
    }
}
