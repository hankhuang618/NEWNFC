<template>
  <section class="board">
    <form class="filters" @submit.prevent="loadData">
      <div class="field">
        <label for="plant">廠區</label>
        <select id="plant" v-model="filters.plant">
          <option v-for="plant in plants" :key="plant.value" :value="plant.value">
            {{ plant.label }}
          </option>
        </select>
      </div>
      <div class="field">
        <label for="division">部</label>
        <select id="division" v-model.number="filters.division">
          <option v-for="value in divisions" :key="value" :value="value">{{ value }}</option>
        </select>
      </div>
      <div class="field">
        <label for="section">課</label>
        <select id="section" v-model.number="filters.section">
          <option v-for="value in sections" :key="value" :value="value">{{ value }}</option>
        </select>
      </div>
      <div class="field">
        <label for="class">班</label>
        <select id="class" v-model.number="filters.classNumber">
          <option v-for="value in classes" :key="value" :value="value">{{ value }}</option>
        </select>
      </div>
      <button type="submit" class="submit">查詢</button>
    </form>

    <div class="content" v-if="error">
      <p class="error">{{ error }}</p>
    </div>

    <div class="content" v-else>
      <div v-if="loading" class="loading">資料讀取中...</div>
      <table v-else class="results">
        <thead>
          <tr>
            <th>部門編號</th>
            <th>部門名稱</th>
            <th>應出席</th>
            <th>實際出席</th>
            <th>在線人數</th>
            <th>請假人數</th>
            <th>借入</th>
            <th>借出</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="records.length === 0">
            <td colspan="8" class="empty">目前查無資料</td>
          </tr>
          <tr v-for="record in records" :key="record.departmentCode">
            <td>{{ record.departmentCode }}</td>
            <td>{{ record.departmentName }}</td>
            <td>{{ record.expectedAttendance }}</td>
            <td>{{ record.actualAttendance }}</td>
            <td>{{ record.onlineCount }}</td>
            <td>{{ record.leaveCount }}</td>
            <td>{{ record.borrowedCount }}</td>
            <td>{{ record.lentCount }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { fetchProductionBoard } from '../services/api';
import type { ProductionBoardFilters, ProductionBoardRecord } from '../types';

const plants = [
  { value: 'ZH', label: '珠海' },
  { value: 'VN', label: '越南' },
  { value: 'TC', label: '太倉' }
];

const divisions = Array.from({ length: 6 }, (_, index) => index + 1);
const sections = Array.from({ length: 4 }, (_, index) => index + 1);
const classes = Array.from({ length: 10 }, (_, index) => index + 1);

const filters = reactive<ProductionBoardFilters>({
  plant: 'ZH',
  division: 1,
  section: 1,
  classNumber: 1
});

const records = ref<ProductionBoardRecord[]>([]);
const loading = ref(false);
const error = ref('');

const loadData = async () => {
  loading.value = true;
  error.value = '';

  try {
    records.value = await fetchProductionBoard(filters);
  } catch (err) {
    console.error(err);
    error.value = '讀取資料時發生問題，請稍後再試或聯絡系統管理員。';
  } finally {
    loading.value = false;
  }
};

onMounted(loadData);
</script>

<style scoped>
.board {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 15px 35px rgba(15, 23, 42, 0.08);
  padding: 2rem;
}

.filters {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
  align-items: end;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

label {
  font-weight: 600;
  color: #334155;
}

select {
  padding: 0.55rem 0.75rem;
  border: 1px solid #cbd5f5;
  border-radius: 8px;
  font-size: 1rem;
  color: #1f2937;
  transition: border-color 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

select:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15);
}

.submit {
  padding: 0.65rem 1rem;
  border: none;
  border-radius: 8px;
  background: linear-gradient(135deg, #2563eb, #1d4ed8);
  color: #ffffff;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.submit:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 20px rgba(37, 99, 235, 0.25);
}

.results {
  width: 100%;
  border-collapse: collapse;
  border-radius: 12px;
  overflow: hidden;
  background: #ffffff;
}

.results thead {
  background: #e2e8f0;
  color: #1e293b;
}

.results th,
.results td {
  padding: 0.85rem 1rem;
  text-align: center;
}

.results tbody tr:nth-child(odd) {
  background: #f8fafc;
}

.results tbody tr:nth-child(even) {
  background: #ffffff;
}

.empty {
  text-align: center;
  color: #64748b;
  font-style: italic;
}

.loading {
  text-align: center;
  font-size: 1.1rem;
  color: #1d4ed8;
}

.error {
  text-align: center;
  color: #dc2626;
  font-weight: 600;
}

@media (max-width: 768px) {
  .results th,
  .results td {
    font-size: 0.9rem;
    padding: 0.75rem;
  }
}
</style>
