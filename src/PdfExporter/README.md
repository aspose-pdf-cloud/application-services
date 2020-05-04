# (C#) PdfExporter library for Web Applications

This library provides PDF reports generation using *Aspose.Pdf.Cloud* and *Aspose.BarCode.Cloud*

## Target Framework

netstandart2.0

## Purpose

Generate PDF documents using *Aspose.Pdf.Cloud* and *Aspose.BarCode.Cloud* based on a report model (*Model.Documen* class from *TemplateExporter* project)

## Classes list

* **PdfReport** (PdfReport.cs) - PDF report generation class that implements *IReport* interface .
* **PdfReport.PdfReportPageProcessor** (*PdfReportPageProcessor.cs*) - Performs PDF page genera
* **IBarcodeApi** (IBarcodeApi.cs)  - Helper interface needed becuase BarcodeApi does not implement any public interface

## Integration

* Create instance

  ```c#
  var pdfReport = new Report.PdfReport(filePath: "Test.pdf");
  ```

* Configure report with previously created instances of PdfApi and Barcode Api

  ```c#
  await pdfReport.Configure(PdfApi, BarcodeApi);
  ```

* Create document model

  ```c#
  var model = new Report.YamlReportModel()
  ```

* Create Document model

  ```c#
  var documentReportModel = model.CreateReportModel(YOUR_DATA_MODEL)
  ```

* Generate report

  ```c#
  await pdfReport.Report(documentReportModel);
  ```

* Upon successful execution you will get *"Test.pdf"* report file ready in Aspose.Storage

