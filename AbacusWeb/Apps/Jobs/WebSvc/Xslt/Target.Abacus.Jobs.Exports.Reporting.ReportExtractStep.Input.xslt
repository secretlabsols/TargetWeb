<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">

    <!--
		Abacus for Windows Job Service - Report Extract step
		
		Formats the ReportExtract input information for display.
	-->
    <xsl:output method="html" indent="no" encoding="UTF-8" />

    <xsl:template match="Inputs">
        <xsl:for-each select="*[local-name() != 'ReportExtractType']">
            <xsl:value-of select="ext:SplitOnCapitals(local-name())"/>: <xsl:value-of select="current()"/>
            <br />
        </xsl:for-each>
    </xsl:template>

</xsl:stylesheet>
