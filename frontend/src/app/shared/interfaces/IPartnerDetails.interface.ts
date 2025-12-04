export interface IPartnerDetails {
  officeName: string;
  partnerName: string;
  partnerPhone: string;
  details: {
    partnerId: string;
    officeId: string;
  };
}
