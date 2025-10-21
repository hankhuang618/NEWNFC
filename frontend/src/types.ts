export interface ProductionBoardFilters {
  plant: 'ZH' | 'VN' | 'TC';
  division: number;
  section: number;
  classNumber: number;
}

export interface ProductionBoardRecord {
  departmentCode: string;
  departmentName: string;
  onlineCount: number;
  expectedAttendance: number;
  actualAttendance: number;
  borrowedCount: number;
  lentCount: number;
  leaveCount: number;
}
