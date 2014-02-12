<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="no" encoding="UTF-8" />
  <!--
		Abacus for Windows Job Service - Create Domiciliary Provider Invoice From Attendance Register
		
		Formats the DomProviderInvoice.CreateFromAttendanceRegister input information for display.
	-->
  <xsl:template match="Inputs">
    Payment Request ID : <xsl:value-of select="PaymentRequestID" /><br />
    <xsl:choose>
      <xsl:when test="string-length(PaymentScheduleRef) &gt; 0">
        Payment Schedule Reference : <xsl:value-of select="PaymentScheduleRef" /><br />
      </xsl:when>
      <xsl:otherwise>
        Pay Up To Date : <xsl:value-of select="PayUpToDate" /> <br />
      </xsl:otherwise>
    </xsl:choose>
    
  </xsl:template>
</xsl:stylesheet>
