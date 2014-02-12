<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Purge Job Service Data
		
		Formats the PurgeJobServiceData.PurgeJobServiceData input information for display.
	-->
    <xsl:template match="Inputs">
        Status Value(s): <xsl:call-template name="StatusValues" />
        <br />
        Completion Date: <xsl:value-of select="FilterCompletionDate" />
    </xsl:template>

    <xsl:template name="StatusValues">
        <xsl:variable name="initialValues" select="ext:SaveToStorage('statusValues', '')" />
        <xsl:for-each select="child::node()">
            <xsl:if test="ext:IndexOf(name(), 'FilterJobStatus') &gt;= 0 and . = 'True'">
                <xsl:variable name="oldValues" select="ext:GetFromStorage('statusValues')" />
                <xsl:variable name="newValues" select="ext:SaveToStorage('statusValues', concat($oldValues, ', ', ext:Replace(name(), 'FilterJobStatus', '')))" />
            </xsl:if>
        </xsl:for-each>
        <xsl:variable name="outputValues" select="ext:GetFromStorage('statusValues')" />
        <xsl:variable name="outputValuesLength" select="string-length($outputValues)" />
        <xsl:if test="$outputValuesLength &gt; 2">
            <xsl:value-of select="ext:Substring($outputValues, 2)" />
        </xsl:if>
    </xsl:template>
    
</xsl:stylesheet>