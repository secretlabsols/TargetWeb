<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
	<xsl:output indent="yes" method="html" encoding="UTF-8" />

    <xsl:key name="lines-by-client" match="StatementData/Data/Line" use="ClientID" />
    <xsl:key name="lines-by-client-and-invoice" match="StatementData/Data/Line" use="concat(ClientID, '+', InvoiceID)" />
	
	<xsl:template match="StatementData">
		
		<table id="detailsTable" cellpadding="0" cellspacing="0">
		<!-- repeated header section -->
		<thead>
			<tr>
				<td colspan="3">
					<!-- logo -->
					<div id="logoContainer">
						<img src="GetSystemLogo.axd" alt="Header logo" />
					</div>
					<!-- council address -->
					<div id="councilAddressContainer">
						<div id="councilName"><xsl:value-of select="Header/Item/SiteName" /></div>
						<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Header/Item/SiteAddress, '&#xA;', '&lt;br/&gt;')" />
					</div>
					<div class="clearer"></div>
				</td>
			</tr>
			<tr>
				<td colspan="3">
					<!-- provider address -->
					<div id="providerAddressContainer">
						<div id="providerName"><xsl:value-of select="Provider/Line/Name" /></div>
						<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Provider/Item/Address, '&#xA;', '&lt;br/&gt;')" />
						<br />
					</div>
                    <div id="providerRefContainer">
                        <xsl:value-of select="concat('Provider Ref: ', Provider/Item/AltReference)" />
                    </div>
                    <div id="currentDateContainer">
                        <xsl:value-of select="concat('Printed: ', ext:CurrentDate())" />
                    </div>
                    <div class="clearer"></div>
				</td>
			</tr>
			<!-- statement title -->
			<tr>
				<td class="statementTitle" colspan="3">
					<xsl:value-of disable-output-escaping="yes" select="concat('Statement of Payments for Domiciliary Contract&lt;br /&gt;&quot;', Contract/Item/Number, '/', Contract/Item/Title, '&quot;')" />
				</td>
			</tr>
			<!-- filters -->
            <xsl:if test="count(Filters/Item) &gt; 0">
                <tr>
                    <td class="filters" colspan="3">
                        <table id="filtersTable" cellpadding="0" cellspacing="0">
                            <xsl:for-each select="Filters/Item">
                                <tr>
                                    <td class="label">
                                        <xsl:value-of disable-output-escaping="yes" select="@Label" />
                                    </td>
                                    <td>
                                        <xsl:value-of disable-output-escaping="yes" select="." />
                                    </td>
                                </tr>
                            </xsl:for-each>                        
                        </table>
                    </td>
                </tr>
            </xsl:if>
			<!-- detail lines header -->
			<tr class="detailHeader">
				<td class="leftMostColumn">Week Ending</td>
				<td class="descColumn">Description</td>
				<td class="rightMostColumn">Amount</td>
			</tr>
		</thead>
		<!-- end of repeated header section -->
		<tbody>

            <!-- for each unique client -->
            <xsl:for-each select="Data/Line[generate-id() = generate-id(key('lines-by-client', ClientID)[1])]">

                <!-- client header -->
                <xsl:call-template name="spacerLine" />
                <tr>
                    <td class="clientHeader" colspan="3">
                        <xsl:value-of disable-output-escaping="yes" select="concat(ClientRef, '/', ClientName)" />
                    </td>
                </tr>

                <!-- for each invoice for this client -->
                <xsl:for-each select="key('lines-by-client', ClientID)
                                            [generate-id() = 
                                            generate-id(key('lines-by-client-and-invoice', 
                                                            concat(ClientID, '+', InvoiceID)))]">
                    
                    <!-- invoice header -->
                    <tr>
                        <td class="invoiceHeader" colspan="3">
                            <xsl:value-of disable-output-escaping="yes" select="InvoiceNumber" />
                        </td>
                    </tr>
                    
                    <!-- invoice lines -->
                    <xsl:for-each select="key('lines-by-client-and-invoice', concat(ClientID, '+', InvoiceID))">
                        <tr>
                            <td class="leftMostColumn">
                                <xsl:value-of select="ext:FormatDateCustom(WeekEnding, 'dd/MM/yyyy')" />
                            </td>
                            <td class="descColumn">
                                <xsl:value-of disable-output-escaping="yes"
                                    select="concat(LineUnits, ' UNITS OF ', ext:ToUpper(Description), ' @ ', ext:FormatCurrency(LineUnitCost))" />
                            </td>
                            <td class="rightMostColumn">
                                <xsl:value-of select="ext:FormatCurrency(LineCost)" />
                            </td>
                        </tr>
                    </xsl:for-each>
                    
                    <!-- invoice total -->
                    <tr>
                        <td class="leftMostColumn invoiceTotalRow">&#160;</td>
                        <td class="descColumn invoiceTotalRow invoiceTotal">
                            Invoice Total
                        </td>
                        <td class="rightMostColumn invoiceTotalRow">
                            <xsl:value-of select="ext:FormatCurrency(sum(key('lines-by-client-and-invoice', concat(ClientID, '+', InvoiceID))/LineCost))" />
                        </td>
                    </tr>
                    
                </xsl:for-each>

                <!-- client total -->
                <tr>
                    <td class="leftMostColumn clientTotalRow">&#160;</td>
                    <td class="clientTotalRow clientTotal">
                        Service User Total
                    </td>
                    <td class="rightMostColumn clientTotalRow">
                        <xsl:value-of select="ext:FormatCurrency(sum(key('lines-by-client', ClientID)/LineCost))" />
                    </td>
                </tr>
                
            </xsl:for-each>
            			
			<!-- statement total -->
            <xsl:call-template name="spacerLine" />
            <tr>
                <td class="leftMostColumn statementTotalRow">&#160;</td>
                <td class="statementTotalRow statementTotal">
                    Statement Total
                </td>
                <td class="rightMostColumn statementTotalRow">
                    <xsl:value-of select="ext:FormatCurrency(sum(Data/Line/LineCost))" />
                </td>
            </tr>
		</tbody>
		<tfoot>
			<!-- repeated footer line -->
			<tr>
				<td class="footer" colspan="3">&#160;</td>
			</tr>
			<!-- end of repeated footer line -->
		</tfoot>
		</table>
		<br />

	</xsl:template>

    <xsl:template name="spacerLine">
        <tr>
            <td class="borderLeftThick borderRightThick" colspan="3">&#160;</td>
        </tr>
    </xsl:template>
		
</xsl:stylesheet>
