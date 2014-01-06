Diga - Distributed Island Genetic Algorithm
====
Requirements
----
* Azure SDK 2.2

Storage
----
* Store [best solution | all solutions] and some metrics (execution time, number of islands, ...)
* Use Table Storage Service: http://www.windowsazure.com/en-us/develop/net/how-to-guides/table-services/
* Webseite fuer Anzeige der Resultate (OWIN/Katana?)

Worker Roles
----
* Use as clients
* [Start only when necessary | Use fixed number which always run]
* http://msdn.microsoft.com/en-us/library/windowsazure/jj149831.aspx
