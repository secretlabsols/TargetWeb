<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:ext="urn:ext-util" exclude-result-prefixes="ext">
	
	<xsl:output indent="yes" method="html" encoding="UTF-8" />
	
	<xsl:template match="RemittanceData">
		
		<table id="detailsTable" cellpadding="0" cellspacing="0">
		<!-- repeated header section -->
		<thead>
			<tr>
				<td colspan="7">
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
				<td colspan="7">
					<!-- home addresses -->
					<div id="homeAddressContainer">
						<div id="homeAddress">
							<div id="homeName"><xsl:value-of select="Data/Line/HomeName" /></div>
							<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Data/Line/Address, '&#xA;', '&lt;br/&gt;')" />
						</div>
					</div>
                    <br />
					<!-- attn of -->
                    <div id="attnOf">Attn: Accounts Manager</div>
                    <!-- creditor ref -->
					<div id="creditorRef">
						<xsl:value-of select="concat('SupplierID: ', Data/Line/CreditorRef)" />
					</div>
                    <div class="clearer"></div>
                    <br />
				</td>
			</tr>
			<!-- remittance title -->
			<tr>
				<td class="remittanceTitle" colspan="7">
					<table cellpadding="0" cellspacing="0" width="100%">
					<tr>
						<td class="remittanceTitleLeft">No. <xsl:value-of select="Data/Line/RemittanceNumber" /></td>
						<td class="remittanceTitleMiddle" colspan="5">REMITTANCE ADVICE</td>
						<td class="remittanceTitleRight"><xsl:value-of select="ext:FormatDateCustom(ext:CurrentDate(), 'dd/MM/yyyy')" /></td>
					</tr>
                    <tr>
                        <td class="remittanceTitleLeft" colspan="3">
                            <xsl:choose>
                                <xsl:when test="string-length(Data/Line/OwnedName) &gt; 0">
                                    <xsl:value-of disable-output-escaping="yes" select="concat('Home: ', Data/Line/HomeName, ' - ', Data/Line/OwnedName)" />
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:value-of disable-output-escaping="yes" select="concat('Home: ', Data/Line/HomeName)" />
                                </xsl:otherwise>
                            </xsl:choose>
                        </td>
                    </tr>
					</table>
				</td>
			</tr>
			<tr>
				<td class="remittanceSubTitle" colspan="7">
					<xsl:value-of select="concat('Proposed payments regarding the period from ', ext:FormatDateCustom(Data/Line/DateFrom, 'dd/MM/yyyy'), ' to ', ext:FormatDateCustom(Data/Line/DateTo, 'dd/MM/yyyy'))" />
				</td>
			</tr>
			<!-- detail lines header -->
			<tr class="detailHeader">
				<td class="leftMostColumn">Client</td>
				<td class="dataColumn">Weeks<br />/Days</td>
				<td class="dataColumn">Gross<br />Rate (£)</td>
				<td class="dataColumn">Direct<br />Income (£)</td>
				<td class="dataColumn">NET Rate<br />(£)</td>
				<td class="dataColumn borderRightThick">Amount<br />(£)</td>
			</tr>
		</thead>
		<!-- end of repeated header section -->
		<tbody>
			<xsl:for-each select="Data/Line">
				<!-- output a line -->
				<xsl:call-template name="remittanceLine" />
                <xsl:if test="ClientRef != following-sibling::Line[1]/ClientRef or position() = last()">
                    <!-- output the net payment line -->
                    <xsl:call-template name="remittanceLineNetPayment" />
                    <!-- output a spacer -->
					<tr>
						<td class="leftMostColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn borderRightThick">&#160;</td>
					</tr>
				</xsl:if>
			</xsl:for-each>
			<!-- VAT footer -->
            <xsl:if test="Data/Line/VATAmount &gt; 0">
                <tr>
                    <td class="leftMostColumn">&#160;</td>
                    <td class="borderTopThin borderLeftThin">&#160;</td>
                    <td class="borderTopThin">&#160;</td>
                    <td class="borderTopThin">&#160;</td>
                    <td class="dataColumn borderTopThin">
                        <strong>VAT %</strong>
                        <br />
                        <xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(Data/Line/VATRate)" />
                    </td>
                    <td class="dataColumn borderTopThin">
                        <strong>Vatable</strong>
                        <br />
                        <xsl:choose>
                            <xsl:when test="Data/Line/VATManualAmount &gt; 0">
                                <xsl:value-of select="ext:FormatCurrency(Data/Line/AmountPaid)" />
                            </xsl:when>
                            <xsl:otherwise>
                                <xsl:value-of select="ext:FormatCurrency(sum(Data/Line[RDVatAmount &gt; 0]/LineValue))" />
                            </xsl:otherwise>
                        </xsl:choose>
                    </td>
                    <td class="dataColumn borderTopThin borderRightThick">
                        <strong>VAT Amt.</strong>
                        <br />
                        <xsl:value-of select="ext:FormatCurrency(Data/Line/VATAmount)" />
                    </td>
                </tr>
            </xsl:if>
			<!-- remittance total -->
			<tr>
				<xsl:attribute name="class"><xsl:if test="SystemInfoSettings/Setting/ShowAuthorisationBoxShading = 'True'">greyBg</xsl:if></xsl:attribute>
				<td class="remittanceTotalTitle" colspan="4">
                    <xsl:choose>
                        <xsl:when test="string-length(Data/Line/OwnedName) &gt; 0">
                            <xsl:value-of disable-output-escaping="yes" 
                                select="concat('For: ', Data/Line/HomeName, ' - ', Data/Line/OwnedName, ', ', Data/Line/CreditorRef, ', ', Data/Line/RemittanceNumber, ', for ', ext:FormatDateCustom(Data/Line/DateFrom, 'dd/MM/yyyy'), ' to ', ext:FormatDateCustom(Data/Line/DateTo, 'dd/MM/yyyy'))" />
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of disable-output-escaping="yes"
                                select="concat('For: ', Data/Line/HomeName, ', ', Data/Line/CreditorRef, ', ', Data/Line/RemittanceNumber, ', for ', ext:FormatDateCustom(Data/Line/DateFrom, 'dd/MM/yyyy'), ' to ', ext:FormatDateCustom(Data/Line/DateTo, 'dd/MM/yyyy'))" />
                        </xsl:otherwise>
                    </xsl:choose>
				</td>
				<td class="remittanceTotalValue" colspan="2">
					<xsl:value-of select="ext:FormatCurrency(sum(Data/Line/LineValue) + Data/Line/VATAmount)" />
				</td>
			</tr>
			<!-- auth text -->
			<tr>
				<xsl:attribute name="class"><xsl:if test="SystemInfoSettings/Setting/ShowAuthorisationBoxShading = 'True'">greyBg</xsl:if></xsl:attribute>
				<td class="borderTopThin borderLeftThick borderRightThick" colspan="6">
					<xsl:value-of disable-output-escaping="yes" select="ext:Replace(SystemInfoSettings/Setting/RemittanceAuthText, '&#xA;', '&lt;br/&gt;')" />
					<br /><br /><br />
					<xsl:value-of disable-output-escaping="yes" select="ext:Replace(SystemInfoSettings/Setting/RemittanceSignOffText, '&#xA;', '&lt;br/&gt;')" />
				</td>
			</tr>
		</tbody>
		<tfoot>
			<!-- repeated footer line -->
			<tr>
				<xsl:attribute name="class"><xsl:if test="SystemInfoSettings/Setting/ShowAuthorisationBoxShading = 'True'">greyBg</xsl:if></xsl:attribute>
				<td class="footer" colspan="6">&#160;</td>
			</tr>
			<!-- end of repeated footer line -->
		</tfoot>
		</table>
		<br />
	
	</xsl:template>
		
	<!-- template to output a line -->
	<xsl:template name="remittanceLine">	
		
		<xsl:choose>
			<xsl:when test="count(../Line[ClientRef = current()/ClientRef]) = 1 and Narrative = 'False'">
                <tr class="dataRow">
					<td class="leftMostColumn">
						<xsl:choose>
							<xsl:when test="position() = 1 or ClientRef != preceding-sibling::Line[1]/ClientRef">
                                <xsl:call-template name="nameAndContractNumber">
                                    <xsl:with-param name="contractNumber" select="ContractNumber" />
                                    <xsl:with-param name="showClientRef">True</xsl:with-param>
                                    <xsl:with-param name="showTeam">False</xsl:with-param>
                                    <xsl:with-param name="clientRef" select="ClientRef" />
                                    <xsl:with-param name="clientName" select="ClientName" />
                                    <xsl:with-param name="teamName" select="TeamName" />
                                </xsl:call-template>
							</xsl:when>
						    <xsl:otherwise>&#160;</xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn">
						<xsl:choose>
							<xsl:when test="string-length(Days) &gt; 0">
								<xsl:value-of disable-output-escaping="yes" select="ext:PrintWeeksDays(Days, '{0}&#32;&#32;{1}')" />
							</xsl:when>
							<xsl:otherwise>&#160;</xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn">
						<xsl:choose>
							<xsl:when test="string-length(GrossRate) = 0">
                                <xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(0)" />
                            </xsl:when>
							<xsl:otherwise><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(GrossRate)" /></xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn">
						<xsl:choose>
							<xsl:when test="string-length(GrossRate) = 0">
                                <xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(0 - NetRate)" />
                            </xsl:when>
							<xsl:otherwise><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(GrossRate - NetRate)" /></xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(NetRate)" /></td>
					<td class="dataColumn borderRightThick"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(LineValue)" /></td>
				</tr>			
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="position() = 1 or ClientRef != preceding-sibling::Line[1]/ClientRef">
					<tr class="dataRow">
						<td class="leftMostColumn">
							<xsl:call-template name="nameAndContractNumber">
								<xsl:with-param name="contractNumber" select="ContractNumber" />
                                <xsl:with-param name="showClientRef">True</xsl:with-param>
                                <xsl:with-param name="showTeam">False</xsl:with-param>
								<xsl:with-param name="clientRef" select="ClientRef" />
								<xsl:with-param name="clientName" select="ClientName" />
								<xsl:with-param name="teamName" select="TeamName" />
							</xsl:call-template>
						</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn borderRightThick">&#160;</td>
					</tr>
				</xsl:if>
				<tr class="dataRow">
					<td class="leftMostColumn" style="padding-left:1.5em;">
					    <xsl:value-of disable-output-escaping="yes" select="ext:Replace(TextLine, '&#xA;', '&lt;br/&gt;')" />&#160;
					</td>
					<td class="dataColumn">
						<xsl:choose>
							<xsl:when test="string-length(Days) &gt; 0">
								<xsl:value-of disable-output-escaping="yes" select="ext:PrintWeeksDays(Days, '{0}&#32;&#32;{1}')" />
							</xsl:when>
							<xsl:otherwise>&#160;</xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn">
						<xsl:choose>
							<xsl:when test="string-length(GrossRate) = 0">
                                <xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(0)" />
                            </xsl:when>
							<xsl:otherwise><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(GrossRate)" /></xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn">
						<xsl:choose>
							<xsl:when test="string-length(GrossRate) = 0">
                                <xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(0 - NetRate)" />
                            </xsl:when>
							<xsl:otherwise><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(GrossRate - NetRate)" /></xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(NetRate)" /></td>
					<td class="dataColumn borderRightThick"><xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(LineValue)" /></td>
				</tr>
			</xsl:otherwise>
		</xsl:choose>
		
	</xsl:template>

    <!-- template to output a line -->
    <xsl:template name="remittanceLineNetPayment">

        <tr class="dataRow">
            <td class="leftMostColumn netPaymentColumn">
                NET Payment
            </td>
            <td class="dataColumn">&#160;</td>
            <td class="dataColumn">&#160;</td>
            <td class="dataColumn">&#160;</td>
            <td class="dataColumn">&#160;</td>
            <td class="dataColumn borderRightThick netPaymentColumn">
                <xsl:value-of select="ext:FormatCurrencyWithoutPoundSign(sum(../Line[ClientRef = current()/ClientRef]/LineValue))" />
            </td>
        </tr>

    </xsl:template>
	
	<!-- template to output the client name/ref/contract number/team name -->
	<xsl:template name="nameAndContractNumber">
		<xsl:param name="contractNumber" />
		<xsl:param name="showClientRef" />
		<xsl:param name="showTeam" />
		<xsl:param name="clientRef" />
		<xsl:param name="clientName" />
		<xsl:param name="teamName" />

        <strong>
		    <xsl:choose>
			    <xsl:when test="string-length($contractNumber) = 0">
				    <xsl:choose>
					    <xsl:when test="$showClientRef = 'True'">
						    <xsl:choose>
							    <xsl:when test="$showTeam = 'True'">
								    <xsl:value-of disable-output-escaping="yes" select="concat($clientName, '&lt;br /&gt;', 'Care First ID: ', $clientRef, '&lt;br /&gt;', $teamName)" />
							    </xsl:when>
							    <xsl:otherwise>
								    <xsl:value-of disable-output-escaping="yes" select="concat($clientName, '&lt;br /&gt;', 'Care First ID: ', $clientRef)" />
							    </xsl:otherwise>
						    </xsl:choose>
					    </xsl:when>
					    <xsl:otherwise>
						    <xsl:choose>
							    <xsl:when test="$showTeam = 'True'">
								    <xsl:value-of disable-output-escaping="yes" select="concat($clientName, '&lt;br /&gt;', $teamName)" />
							    </xsl:when>
							    <xsl:otherwise>
								    <xsl:value-of disable-output-escaping="yes" select="$clientName" />
							    </xsl:otherwise>
						    </xsl:choose>
					    </xsl:otherwise>
				    </xsl:choose>
			    </xsl:when>
			    <xsl:otherwise>
				    <xsl:choose>
					    <xsl:when test="$showClientRef = 'True'">
						    <xsl:choose>
							    <xsl:when test="$showTeam = 'True'">
								    <xsl:value-of disable-output-escaping="yes" select="concat($clientName, ' (', $contractNumber, ')', '&lt;br /&gt;', 'Care First ID: ', $clientRef, '&lt;br /&gt;', $teamName)" />
							    </xsl:when>
							    <xsl:otherwise>
								    <xsl:value-of disable-output-escaping="yes" select="concat($clientName, ' (', $contractNumber, ')', '&lt;br /&gt;', 'Care First ID: ', $clientRef)" />
							    </xsl:otherwise>
						    </xsl:choose>
					    </xsl:when>
					    <xsl:otherwise>
						    <xsl:choose>
							    <xsl:when test="$showTeam = 'True'">
								    <xsl:value-of disable-output-escaping="yes" select="concat($clientName, ' (', $contractNumber, ')', '&lt;br /&gt;', $teamName)" />
							    </xsl:when>
							    <xsl:otherwise>
								    <xsl:value-of disable-output-escaping="yes" select="concat($clientName, ' (', $contractNumber, ')')" />
							    </xsl:otherwise>
						    </xsl:choose>
					    </xsl:otherwise>
				    </xsl:choose>
			    </xsl:otherwise>
		    </xsl:choose>
        </strong>
		
	</xsl:template>
	
</xsl:stylesheet>
