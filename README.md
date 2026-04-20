<h1 align="center">Get-CurrentEduAssignment</h1>

<p align="center">
A simple pair of CLI scripts for reviewing currently due Microsoft Teams assignments in your Edu tenant.
</p>

---

## Requirements

These scripts require:

* A **Microsoft Entra Edu tenant**
* An **App registration** with the following **admin-consented permissions**:

  * `EduAssignment.Read.All`
  * `EduRoster.Read.All`
  * `User.Read.All`

---

## Configuration (`ScriptValues.json`)

This file must be populated with values specific to your environment:

* **TenantId**
  Your tenant’s unique ID (found in the Entra portal)

* **AppId**
  Your application (client) ID (from the app overview)

* **ClientSecret**
  Your application secret value (from **Certificates & Secrets**)

* **DefaultDomain**
  Your domain (used only in the attended script)

* **OutputFolder**
  Folder where CSV output will be saved (used only in the unattended script)