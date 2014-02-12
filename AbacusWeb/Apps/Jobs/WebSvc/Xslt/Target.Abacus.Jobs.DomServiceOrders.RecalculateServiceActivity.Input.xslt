<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="no" encoding="UTF-8" />
	<!--
		Abacus for Windows Job Service - Dom Service Orders - Recalc Service Activity.
		
		Formats the DomServiceOrders.RecalculateServiceActivity input information for display.
	-->

    <xsl:template match="/">
        <xsl:choose>
            <xsl:when test="Inputs">
                    <b>Selected Service User: </b>
                    <xsl:choose>
                        <xsl:when test="string-length(Inputs/FilterServiceUser) &gt; 0">
                            <xsl:value-of select="Inputs/FilterServiceUser" />
                        </xsl:when>
                        <xsl:otherwise>
                            All Service Users
                        </xsl:otherwise>
                    </xsl:choose>
                    <br />
                    <b>Selected Period From: </b>
                    <xsl:choose>
                        <xsl:when test="string(Inputs/FilterPeriodFrom) = '01-Jan-1900'">
                            No Limit
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of select="Inputs/FilterPeriodFrom" />
                        </xsl:otherwise>
                    </xsl:choose>
                    <br />
                    <b>Selected Period To: </b>
                    <xsl:choose>
                        <xsl:when test="string(Inputs/FilterPeriodTo) = '31-Dec-9999'">
                            No Limit
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of select="Inputs/FilterPeriodTo" />
                        </xsl:otherwise>
                    </xsl:choose>
                    <br />
                    <b>Force Reconsideration: </b>
                    <xsl:choose>
                        <xsl:when test="string(Inputs/FilterForceReconsideration) = 'True'">
                            Yes
                        </xsl:when>
                        <xsl:otherwise>
                            No
                        </xsl:otherwise>
                    </xsl:choose>
            </xsl:when>
            <xsl:otherwise>
                <b>Selected Service User: </b> All Service Users
                <br />
                <b>Selected Period From: </b> No Limit
                <br />
                <b>Selected Period To: </b> No Limit
                <br />
                <b>Force Reconsideration: </b> No
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
	

</xsl:stylesheet>
