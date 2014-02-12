<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Create SDS notification Letters
		
		Formats the output for the step
	-->
    <xsl:template match="Results">

        Total No. of notifications sucessfully created: <xsl:value-of select="NotificationsToCreate" /><br />
        Total No. of notifications failing creation: <xsl:value-of select="NotificationsFailed" /><br />    

        <xsl:if test="(NotificationsToCreate + NotificationsFailed) &gt; 0">
            <br /><br />
            Please see the action report for more details:
            <br />
            View the report <a href="{ActionReportCsvURL}">in Excel</a>
            <br />
            View the report <a href="{ActionReportXmlURL}">as XML</a>
        </xsl:if>

     
    </xsl:template>
</xsl:stylesheet>