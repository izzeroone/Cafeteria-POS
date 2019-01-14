using System;
using Cafocha.Repository.DAL;
using PdfRpt.Core.Contracts;

namespace Cafocha.GUI.Helper.PrintHelper.Report
{
    public interface IListPdfReport
    {
        IPdfReportData CreatePdfReport(RepositoryLocator unitofwork, DateTime startime, DateTime endTime,
            string folderName);

        IPdfReportData CreateDetailsPdfReport(RepositoryLocator unitofwork, DateTime startime, DateTime endTime,
            string folderName);

        IPdfReportData CreateEntityPdfReport(RepositoryLocator unitofwork, DateTime startime, DateTime endTime,
            string folderName);

        IPdfReportData CreateMonthPdfReport(RepositoryLocator unitofwork, string folderName);

        IPdfReportData CreateDayPdfReport(RepositoryLocator unitofwork, string folderName);

        IPdfReportData CreateYearPdfReport(RepositoryLocator unitofwork, string folderName);
    }
}