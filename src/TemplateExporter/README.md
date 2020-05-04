# (C#) TemplateExporter library for Web Applications

This library provides template reporting implementation based on Yaml documents

## Target Framework

netstandart2.1

## Purpose

Use Yaml documents to develop reporting model

## Classes list

* **IReport** (*Interface/IReport.cs*) - Reporting interface.
* **YamlReportModel** (*YamlReportModel.cs*) - Allows to create *Model.Document* object based on a Yaml content and data model provided
* *ReportModel* folder contains DTO classes that represent reporting model

## Integration

* Your reporting library should implement *IReport* interface and use *YamlReportModel* to build *Model.Document*
* For reference see [PdfExporter](../PdfExporter/README.md)