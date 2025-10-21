<template>
  <main class="container">
    <header class="header">
      <h1>在線生產看板</h1>
      <p class="subtitle">選擇廠區與部門條件即可查看即時人員狀態</p>
    </header>
    <ProductionBoard />
  </main>
</template>

<script setup lang="ts">
import ProductionBoard from './components/ProductionBoard.vue';
</script>

<style scoped>
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1.5rem 4rem;
}

.header {
  text-align: center;
  margin-bottom: 2rem;
}

.header h1 {
  margin: 0;
  font-size: 2.25rem;
  color: #1c3d5a;
}

.subtitle {
  margin-top: 0.5rem;
  color: #52606d;
}
</style>
