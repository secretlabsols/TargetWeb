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
						<div id="ownedAddress">
							<div id="ownedHomeName">
								<xsl:if test="string-length(Data/Line/OwnedName) &gt; 0">
									<xsl:value-of select="concat('For: ', Data/Line/OwnedName)" />
								</xsl:if>
							</div>
							<xsl:value-of disable-output-escaping="yes" select="ext:Replace(Data/Line/OwnedAddress, '&#xA;', '&lt;br/&gt;')" />
						</div>
						<div class="clearer"></div>
					</div>
					<!-- creditor ref -->
					<xsl:if test="SystemInfoSettings/Setting/ShowCreditorRefOnRemittance = 'True'">
						<div id="creditorRef">
							<xsl:value-of select="concat('Creditor Ref: ', Data/Line/CreditorRef)" />
						</div>
					</xsl:if>
				</td>
			</tr>
			<!-- remittance title -->
			<tr>
				<td class="remittanceTitle" colspan="7">
					<table cellpadding="0" cellspacing="0" width="100%">
					<tr>
						<td class="remittanceTitleLeft">No. <xsl:value-of select="Data/Line/RemittanceNumber" /></td>
						<td class="remittanceTitleMiddle" colspan="5"><xsl:value-of select="SystemInfoSettings/Setting/RemittanceHeaderTitle" /></td>
						<td class="remittanceTitleRight"><xsl:value-of select="ext:FormatDateCustom(ext:CurrentDate(), 'dd/MM/yyyy')" /></td>
					</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td class="remittanceSubTitle" colspan="7">
					<xsl:value-of select="concat('Payments re: Residential &amp; Nursing Placements Relating to the Period from ', ext:FormatDateCustom(Data/Line/DateFrom, 'dd/MM/yyyy'), ' to ', ext:FormatDateCustom(Data/Line/DateTo, 'dd/MM/yyyy'))" />
				</td>
			</tr>
			<!-- detail lines header -->
			<tr class="detailHeader">
				<td class="leftMostColumn">Client</td>
				<td class="dataColumn">Weeks<br />/Days</td>
				<td class="dataColumn">Gross<br />Rate</td>
				<td class="dataColumn">Direct<br />Income</td>
				<td class="dataColumn">NET<br />Rate</td>
				<td class="dataColumn">Amount</td>
				<td class="dataColumn borderRightThick">NET<br />Payment</td>
			</tr>
		</thead>
		<!-- end of repeated header section -->
		<tbody>
			<xsl:for-each select="Data/Line">
				<!-- output a line -->
				<xsl:call-template name="remittanceLine" />
				<!-- output a spacer -->
				<xsl:if test="ClientRef != following-sibling::Line[1]/ClientRef or position() = last()">
					<tr>
						<td class="leftMostColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
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
				<td class="remittanceTotalTitle" colspan="5">Total Remittance</td>
				<td class="remittanceTotalValue" colspan="2">
					<xsl:value-of select="ext:FormatCurrency(sum(Data/Line/LineValue) + Data/Line/VATAmount)" />
				</td>
			</tr>
			<!-- auth text -->
			<tr>
				<xsl:attribute name="class"><xsl:if test="SystemInfoSettings/Setting/ShowAuthorisationBoxShading = 'True'">greyBg</xsl:if></xsl:attribute>
				<td class="borderTopThin borderLeftThick borderRightThick" colspan="7">
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
				<td class="footer" colspan="7">&#160;</td>
			</tr>
			<!-- end of repeated footer line -->
		</tfoot>
		</table>
		<br />
	
	</xsl:template>
		
	<!-- template to output a line -->
	<xsl:template name="remittanceLine">	
		
		<xsl:choose>
			<xsl:when test="Narrative = 'False'">
				<tr class="dataRow">
					<td class="leftMostColumn">
						<xsl:choose>
							<xsl:when test="position() = 1 or ClientRef != preceding-sibling::Line[1]/ClientRef">
								<xsl:call-template name="nameAndContractNumber">
									<xsl:with-param name="contractNumber" select="ContractNumber" />
									<xsl:with-param name="showClientRef" select="ShowClientRefOnRemittance" />
									<xsl:with-param name="showTeam" select="ShowTeamOnRemittance" />
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
							<xsl:when test="string-length(GrossRate) = 0">&#160;</xsl:when>
							<xsl:otherwise><xsl:value-of select="ext:FormatCurrency(GrossRate)" /></xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn">
						<xsl:choose>
							<xsl:when test="string-length(GrossRate) = 0">&#160;</xsl:when>
							<xsl:otherwise><xsl:value-of select="ext:FormatCurrency(GrossRate - NetRate)" /></xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn"><xsl:value-of select="ext:FormatCurrency(NetRate)" /></td>
					<td class="dataColumn"><xsl:value-of select="ext:FormatCurrency(LineValue)" /></td>
					<td class="dataColumn borderRightThick">
						<xsl:choose>
							<xsl:when test="string(ClientRef) = string(following-sibling::Line/ClientRef)">&#160;</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="ext:FormatCurrency(sum(../Line[ClientRef = current()/ClientRef]/LineValue))" />
							</xsl:otherwise>
						</xsl:choose>
					</td>
				</tr>			
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="position() = 1 or ClientRef != preceding-sibling::Line[1]/ClientRef">
					<tr class="dataRow">
						<td class="leftMostColumn">
							<xsl:call-template name="nameAndContractNumber">
								<xsl:with-param name="contractNumber" select="ContractNumber" />
								<xsl:with-param name="showClientRef" select="ShowClientRefOnRemittance" />
								<xsl:with-param name="showTeam" select="ShowTeamOnRemittance" />
								<xsl:with-param name="clientRef" select="ClientRef" />
								<xsl:with-param name="clientName" select="ClientName" />
								<xsl:with-param name="teamName" select="TeamName" />
							</xsl:call-template>
						</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn">&#160;</td>
						<td class="dataColumn borderRightThick">&#160;</td>
					</tr>
				</xsl:if>
				<tr class="dataRow">
					<td class="leftMostColumn" style="padding-left:1.5em;"><xsl:value-of disable-output-escaping="yes" select="ext:Replace(TextLine, '&#xA;', '&lt;br/&gt;')" /></td>
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
							<xsl:when test="string-length(GrossRate) = 0">&#160;</xsl:when>
							<xsl:otherwise><xsl:value-of select="ext:FormatCurrency(GrossRate)" /></xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn">
						<xsl:choose>
							<xsl:when test="string-length(GrossRate) = 0">&#160;</xsl:when>
							<xsl:otherwise><xsl:value-of select="ext:FormatCurrency(GrossRate - NetRate)" /></xsl:otherwise>
						</xsl:choose>
					</td>
					<td class="dataColumn"><xsl:value-of select="ext:FormatCurrency(NetRate)" /></td>
					<td class="dataColumn"><xsl:value-of select="ext:FormatCurrency(LineValue)" /></td>
					<td class="dataColumn borderRightThick">
						<xsl:choose>
							<xsl:when test="ClientRef = following-sibling::Line[1]/ClientRef">&#160;</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="ext:FormatCurrency(sum(../Line[ClientRef = current()/ClientRef]/LineValue))" />
							</xsl:otherwise>
						</xsl:choose>
					</td>
				</tr>
			</xsl:otherwise>
		</xsl:choose>
		
	</xsl:template>
	
	<!-- template to output the client name/ref/contract number/team name -->
	<xsl:template name="nameAndContractNumber">
		<xsl:param name="contractNumber" />
		<xsl:param name="showClientRef" />
		<xsl:param name="showTeam" />
		<xsl:param name="clientRef" />
		<xsl:param name="clientName" />
		<xsl:param name="teamName" />
	
		<xsl:choose>
			<xsl:when test="string-length($contractNumber) = 0">
				<xsl:choose>
					<xsl:when test="$showClientRef = 'True'">
						<xsl:choose>
							<xsl:when test="$showTeam = 'True'">
								<xsl:value-of disable-output-escaping="yes" select="concat($clientName, '&lt;br /&gt;', 'Client Ref: ', $clientRef, '&lt;br /&gt;', $teamName)" />
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of disable-output-escaping="yes" select="concat($clientName, '&lt;br /&gt;', 'Client Ref: ', $clientRef)" />
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
								<xsl:value-of disable-output-escaping="yes" select="concat($clientName, ' (', $contractNumber, ')', '&lt;br /&gt;', 'Client Ref: ', $clientRef, '&lt;br /&gt;', $teamName)" />
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of disable-output-escaping="yes" select="concat($clientName, ' (', $contractNumber, ')', '&lt;br /&gt;', 'Client Ref: ', $clientRef)" />
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
		
	</xsl:template>
	
</xsl:stylesheet>
