<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Debtors Interface - Create Recurring Input.
		
		Formats the Exports.FinancialExportInterface.Debtors.CreateRecurring input information for display.
	-->
    <xsl:template match="SavedWizardSelection">
        Saved Wizard Selection Name: <xsl:value-of select="." /><br />
	</xsl:template>
    
</xsl:stylesheet>
