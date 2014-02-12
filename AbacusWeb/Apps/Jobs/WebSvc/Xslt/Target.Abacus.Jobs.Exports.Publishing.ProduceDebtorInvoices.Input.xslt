<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
		Abacus for Windows Job Service - Publishing Interface
		
		Formats the Publishing.ProduceDebtorInvoices input information for display.
	-->
    <xsl:template match="Inputs">
      DEBTOR INVOICES:<br />
      <xsl:choose>
        <xsl:when test="string-length(SelectUnprintedInvoices) &gt; 0 and SelectUnprintedInvoices = 'True'">
          Selection Type: <b>Unprinted Invoices</b><br />
          Billing Type: <b><xsl:value-of select="string(BillingTypeDesc)" /></b><br />
        </xsl:when>
        <xsl:otherwise>
          Selection Type: <b>Single Invoice</b><br />
          Selected Invoice: <b><xsl:value-of select="string(InvoiceNumber)" /></b><br />
        </xsl:otherwise>
      </xsl:choose>
      <br />
      TRANSACTION STATEMENTS:<br />
      <xsl:choose>
        <xsl:when test="string-length(ProduceStatements) &gt; 0 and ProduceStatements = 'True'">
          Produce Transaction Statements?: <b>Yes</b><br />
          Transactions From: <b><xsl:value-of select="ext:FormatDateCustom(string(TransactionsDateFrom), 'dd/MM/yyyy')" /></b><br />
          Transactions To: <b><xsl:value-of select="ext:FormatDateCustom(string(TransactionsDateTo), 'dd/MM/yyyy')" /></b><br />
          Statement Footer Text: <b><xsl:value-of select="string(StatementFooterText)" /></b><br />
          <xsl:choose>
            <xsl:when test="string-length(ReplaceStandardText) &gt; 0 and ReplaceStandardText = 'True'">
              Replace standard text?: <b>Yes</b><br />
            </xsl:when>
            <xsl:otherwise>
              Replace standard text?: <b>No</b><br />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:otherwise>
          Produce Transaction Statements?: <b>No</b><br />
        </xsl:otherwise>
      </xsl:choose>
      <br />
      DOCUMENTATION SORTING:<br />
      <xsl:choose>
        <xsl:when test="string-length(DocumentSortOrderDesc) &gt; 0">
          Sorted By: <b><xsl:value-of select="string(DocumentSortOrderDesc)" /></b><br />
        </xsl:when>
        <xsl:otherwise>
          Sorted By: <b>Reference</b><br />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:template>
    
</xsl:stylesheet>